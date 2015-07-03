using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace OneAppAway
{
    public static class Data
    {
        public const double MAX_LAT_RANGE = 0.01;
        public const double MAX_LON_RANGE = 0.015;

        public static async Task<BusStop[]> GetStopsForArea(GeoboundingBox bounds, Action<BusStop[], GeoboundingBox> stopsLoadedCallback, CancellationToken cancellationToken)
        {
            List<BusStop> totalFoundStops = new List<BusStop>();
            double latRange = bounds.NorthwestCorner.Latitude - bounds.SoutheastCorner.Latitude;
            double lonRange = bounds.SoutheastCorner.Longitude - bounds.NorthwestCorner.Longitude;
            int latPieces = (int)Math.Ceiling(latRange / MAX_LAT_RANGE);
            int lonPieces = (int)Math.Ceiling(lonRange / MAX_LON_RANGE);
            double smallLatRange = latRange / (double)latPieces;
            double smallLonRange = lonRange / (double)lonPieces;
            for (int i = 0; i < latPieces; i++)
            {
                for (int j = 0; j < lonPieces; j++)
                {
                    BusStop[] foundStops = await ApiLayer.GetBusStops(new BasicGeoposition() { Latitude = bounds.SoutheastCorner.Latitude + (i + .5) * smallLatRange, Longitude = bounds.NorthwestCorner.Longitude + (j + .5) * smallLonRange }, smallLatRange, smallLonRange, cancellationToken);
                    foreach (var item in foundStops)
                    {
                        totalFoundStops.Add(item);
                    }
                    if (stopsLoadedCallback != null)
                    {
                        stopsLoadedCallback(foundStops, null);
                    }
                }
            }
            return totalFoundStops.ToArray();
        }
    }
}
