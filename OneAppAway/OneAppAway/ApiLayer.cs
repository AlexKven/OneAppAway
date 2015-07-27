using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Devices.Geolocation;

namespace OneAppAway
{
    public static class ApiLayer
    {
        public static async Task<string> SendRequest(string compactRequest, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            HttpClient client = new HttpClient();
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.pugetsound.onebusaway.org/api/where/" + compactRequest + ".xml?key=" + Keys.ObaKey + parameters?.Aggregate("", (acc, item) => acc + "&" + item.Key + "=" + item.Value) ?? ""), cancellationToken);
            if (cancellationToken.IsCancellationRequested) return null;
            return await resp.Content.ReadAsStringAsync();
        }

        public static async Task<string> SendRequest(string compactRequest, Dictionary<string, string> parameters)
        {
            return await SendRequest(compactRequest, parameters, new CancellationToken());
        }

        public static async Task<BusStop[]> GetBusStops(BasicGeoposition center, double latSpan, double lonSpan, CancellationToken cancellationToken)
        {
            List<BusStop> result = new List<BusStop>();
            var responseString = await SendRequest("stops-for-location", new Dictionary<string, string>() { ["lat"] = center.Latitude.ToString(), ["lon"] = center.Longitude.ToString(), ["latSpan"] = latSpan.ToString(), ["lonSpan"] = lonSpan.ToString() });

            StringReader reader = new StringReader(responseString);
            XDocument xDoc = XDocument.Load(reader);

            //XElement el = (XElement)xDoc.Nodes().First(d => d.NodeType == XmlNodeType.Element && ((XElement)d).Name.LocalName == "response");
            XElement el = xDoc.Element("response").Element("data");
            XElement elList = el.Element("list");

            foreach (XElement el1 in elList.Elements("stop"))
            {
                string lat = el1.Element("lat")?.Value;
                string lon = el1.Element("lon")?.Value;
                string direction = el1.Element("direction")?.Value;
                string name = el1.Element("name")?.Value;
                string code = el1.Element("code")?.Value;
                string id = el1.Element("id")?.Value;
                string locationType = el1.Element("locationType")?.Value;
                List<string> routeIds = new List<string>();
                foreach (XElement el2 in el1.Element("routeIds").Elements("string"))
                {
                    routeIds.Add(el2?.Value);
                }
                BusStop stop = new BusStop() { Position = new BasicGeoposition() { Latitude = double.Parse(lat), Longitude = double.Parse(lon) }, Direction = direction == null ? StopDirection.Unspecified : (StopDirection)Enum.Parse(typeof(StopDirection), direction), Name = name, ID = id, Code = code, LocationType = int.Parse(locationType), Routes = routeIds.ToArray() };
                result.Add(stop);
            }

            XElement elRoutes = el.Element("references").Element("routes");
            return result.ToArray();
        }

        public static async Task<BusArrival[]> GetBusArrivals(string id)
        {
            List<BusArrival> result = new List<BusArrival>();
            StringReader reader = new StringReader(await SendRequest("arrivals-and-departures-for-stop/" + id, null));
            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response");
            el = el.Element("data");
            el = el.Element("entry");
            el = el.Element("arrivalsAndDepartures");

            foreach (XElement el1 in el.Elements("arrivalAndDeparture"))
            {
                string routeName = el1.Element("routeShortName") == null ? el1.Element("routeLongName").Value : el1.Element("routeShortName").Value;
                string routeId = el1.Element("routeId")?.Value;
                string tripId = el1.Element("tripId")?.Value;
                string stopId = el1.Element("stopId")?.Value;
                string predictedArrivalTime = el1.Element("predictedArrivalTime")?.Value;
                string scheduledArrivalTime = el1.Element("scheduledArrivalTime")?.Value;
                string lastUpdateTime = el1.Element("lastUpdateTime")?.Value;
                string destination = el1.Element("tripHeadsign")?.Value;
                long predictedArrivalLong = long.Parse(predictedArrivalTime);
                long scheduledArrivalLong = long.Parse(scheduledArrivalTime);
                long? lastUpdateLong = lastUpdateTime == null ? null : new long?(long.Parse(scheduledArrivalTime));
                DateTime? predictedArrival = predictedArrivalLong == 0 ? null : new DateTime?((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(predictedArrivalLong)).ToLocalTime());
                DateTime scheduledArrival = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(scheduledArrivalLong)).ToLocalTime();
                DateTime? lastUpdate = lastUpdateLong == null ? null : new DateTime?((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(lastUpdateLong.Value)).ToLocalTime());
                BusArrival arrival = new BusArrival() { RouteName = routeName, PredictedArrivalTime = predictedArrival, ScheduledArrivalTime = scheduledArrival, LastUpdateTime = lastUpdate, Route = routeId, Trip = tripId, Stop = stopId, Destination = destination };
                result.Add(arrival);
            }

            return result.ToArray();
        }

        public static async Task<BusStop> GetBusStop(string id)
        {
            StringReader reader = new StringReader(await SendRequest("stop/" + id, null));
            XDocument xDoc = XDocument.Load(reader);

            var sched = await SendRequest("schedule-for-stop/" + id, new Dictionary<string, string>() {["date"] = "2015-07-16" });

            XElement el = xDoc.Element("response").Element("data").Element("entry");

            string lat = el.Element("lat")?.Value;
            string lon = el.Element("lon")?.Value;
            string direction = el.Element("direction")?.Value;
            string name = el.Element("name")?.Value;
            string code = el.Element("code")?.Value;
            string locationType = el.Element("locationType")?.Value;
            List<string> routeIds = new List<string>();
            foreach (XElement el2 in el.Element("routeIds").Elements("string"))
            {
                routeIds.Add(el2?.Value);
            }
            return new BusStop() { Position = new BasicGeoposition() { Latitude = double.Parse(lat), Longitude = double.Parse(lon) }, Direction = direction == null ? StopDirection.Unspecified : (StopDirection)Enum.Parse(typeof(StopDirection), direction), Name = name, ID = id, Code = code, LocationType = int.Parse(locationType), Routes = routeIds.ToArray() };
        }

        public static async Task<BusRoute> GetBusRoute(string id)
        {
            StringReader reader = new StringReader(await SendRequest("route/" + id, null));
            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry");
            string routeId = el.Element("id")?.Value;
            var elDescription = el.Element("description");
            var elShortName = el.Element("shortName");
            var elLongName = el.Element("longName");
            string routeName = elShortName == null ? elLongName == null ? "(No Name)" : elLongName.Value : elShortName.Value;
            string routeDescription = elDescription == null ? elLongName == null ? elShortName == null ? "No Description" : elShortName.Value : elLongName.Value : elDescription.Value;
            string routeAgency = el.Element("agencyId")?.Value;
            return new BusRoute() { ID = routeId, Name = routeName, Description = routeDescription, Agency = routeAgency };
        }

        public static async Task<TransitAgency> GetTransitAgency(string id)
        {
            StringReader reader = new StringReader(await SendRequest("agency/" + id, null));
            
            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry");
            string agencyName = el.Element("name")?.Value;
            string agencyUrl = el.Element("url")?.Value;
            return new TransitAgency() { Id = id, Name = agencyName, Url = agencyUrl};
        }

        public static async Task<BusTrip[]> GetTripsForRoute(string route)
        {
            List<string> tripIds = new List<string>();

            StringReader reader = new StringReader(await SendRequest("trips-for-route/" + route, null));
            XDocument xDoc = XDocument.Load(reader);

            foreach (var el in xDoc.Element("response").Element("data").Element("list").Elements("tripDetails"))
            {
                tripIds.Add(el.Element("tripId")?.Value);
            }

            List<BusTrip> result = new List<BusTrip>();

            foreach (var tripId in tripIds)
            {
                reader = new StringReader(await SendRequest("trip/" + tripId, null));
                xDoc = XDocument.Load(reader);
                var el = xDoc.Element("response").Element("data").Element("entry");
                string shapeId = el.Element("shapeId")?.Value;
                string routeId = el.Element("routeId")?.Value;
                string destination = el.Element("tripHeadsign")?.Value;
                if (routeId == route)
                    result.Add(new BusTrip() { Route = routeId, Shape = shapeId, Destination = destination });
            }

            return result.ToArray();
        }

        public static async Task<string> GetShape(string id)
        {
            StringReader reader = new StringReader(await SendRequest("shape/" + id, null));

            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry");
            return el.Element("points")?.Value;
        }
    }
}
