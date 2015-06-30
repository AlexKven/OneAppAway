using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace OneAppAway
{
    public static class Data
    {
        public const double MAX_LAT_RANGE = 0.01;
        public const double MAX_LON_RANGE = 0.015;

        public static async Task<BusStop[]> GetStopsForArea(BasicGeoposition topleft, BasicGeoposition bottomRight, Action<BusStop[]> stopsLoadedCallback = null)
        {
            List<BusStop> totalFoundStops = new List<BusStop>();
            double latRange = topleft.Latitude - bottomRight.Latitude;
            double lonRange = bottomRight.Longitude - topleft.Longitude;
            int latPieces = (int)Math.Ceiling(latRange / MAX_LAT_RANGE);
            int lonPieces = (int)Math.Ceiling(lonRange / MAX_LON_RANGE);
            double smallLatRange = latRange / (double)latPieces;
            double smallLonRange = lonRange / (double)lonPieces;
            for (int i = 0; i < latPieces; i++)
            {
                for (int j = 0; j < lonPieces; j++)
                {
                    BusStop[] foundStops = await ApiLayer.GetBusStops(new BasicGeoposition() { Latitude = bottomRight.Latitude + (i + .5) * smallLatRange, Longitude = topleft.Longitude + (j + .5) * smallLonRange }, smallLatRange, smallLonRange);
                    foreach (var item in foundStops)
                    {
                        totalFoundStops.Add(item);
                    }
                    if (stopsLoadedCallback != null)
                    {
                        stopsLoadedCallback(foundStops);
                    }
                }
            }
            return totalFoundStops.ToArray();
        }
    }
}
