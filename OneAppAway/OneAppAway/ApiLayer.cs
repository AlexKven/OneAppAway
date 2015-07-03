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
            XElement el = xDoc.Element("response");

            el = el.Element("data");
            el = el.Element("list");

            foreach (XElement el1 in el.Elements("stop"))
            {
                string lat = el1.Element("lat").Value;
                string lon = el1.Element("lon").Value;
                string direction = el1.Element("direction") == null ? null : el1.Element("direction").Value;
                string name = el1.Element("name").Value;
                string code = el1.Element("code").Value;
                string id = el1.Element("id").Value;
                string locationType = el1.Element("locationType").Value;
                List<string> routeIds = new List<string>();
                foreach (XElement el2 in el1.Element("routeIds").Elements("string"))
                {
                    routeIds.Add(el2.Value);
                }
                BusStop stop = new BusStop() { Position = new BasicGeoposition() { Latitude = double.Parse(lat), Longitude = double.Parse(lon) }, Direction = direction == null ? StopDirection.Unspecified : (StopDirection)Enum.Parse(typeof(StopDirection), direction), Name = name, ID = id, Code = code, LocationType = int.Parse(locationType), RouteIds = routeIds.ToArray() };
                result.Add(stop);
            }
            Debug.WriteLine(result.Count);
            return result.ToArray();
        }
    }
}
