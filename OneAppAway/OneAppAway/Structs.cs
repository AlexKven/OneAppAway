using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace OneAppAway
{
    public enum StopDirection { Unspecified, N, NE, E, SE, S, SW, W, NW }

    public struct BusStop
    {
        public StopDirection Direction { get; set; }
        public BasicGeoposition Position { get; set; }
        public string ID { get; set; }
        public string FriendlyName { get; set; }
    }
}
