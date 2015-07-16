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
        public static async Task<BusStop[]> GetBusStops(BasicGeoposition center, double latSpan, double lonSpan, CancellationToken cancellationToken)
        {
            List<BusStop> result = new List<BusStop>();
            HttpClient client = new HttpClient();
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.pugetsound.onebusaway.org/api/where/stops-for-location.xml?key=TEST&lat=" + center.Latitude.ToString() + "&lon=" + center.Longitude.ToString() + "&latSpan=" + latSpan.ToString() + "&lonSpan=" + lonSpan.ToString()), cancellationToken);
            if (cancellationToken.IsCancellationRequested) return new BusStop[0];

            var responseString = await resp.Content.ReadAsStringAsync();

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
            Debug.WriteLine(result.Count);
            return result.ToArray();
        }

        public static async Task<BusArrival[]> GetBusArrivals(string id)
        {
            List<BusArrival> result = new List<BusArrival>();
            HttpClient client = new HttpClient();
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.pugetsound.onebusaway.org/api/where/arrivals-and-departures-for-stop/" + id + ".xml?key=TEST"));

            var responseString = await resp.Content.ReadAsStringAsync();

            StringReader reader = new StringReader(responseString);
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
                BusArrival arrival = new BusArrival() { RouteName = routeName, PredictedArrivalTime = predictedArrival, ScheduledArrivalTime = scheduledArrival, LastUpdateTime = lastUpdate, RouteID = routeId, TripID = tripId, StopID = stopId, Destination = destination };
                result.Add(arrival);
            }

            return result.ToArray();
        }

        public static async Task<BusRoute> GetBusRoute(string id)
        {
            List<BusArrival> result = new List<BusArrival>();
            HttpClient client = new HttpClient();
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.pugetsound.onebusaway.org/api/where/route/" + id + ".xml?key=TEST"));

            var responseString = await resp.Content.ReadAsStringAsync();

            StringReader reader = new StringReader(responseString);
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
            List<BusArrival> result = new List<BusArrival>();
            HttpClient client = new HttpClient();
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.pugetsound.onebusaway.org/api/where/agency/" + id + ".xml?key=TEST"));

            var responseString = await resp.Content.ReadAsStringAsync();

            StringReader reader = new StringReader(responseString);
            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry");
            string agencyName = el.Element("name")?.Value;
            string agencyUrl = el.Element("url")?.Value;
            return new TransitAgency() { Id = id, Name = agencyName, Url = agencyUrl};
        }
    }
}
