using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace OneAppAway
{
    enum StopDirection { N, NE, E, SE, S, SW, W, NW }

    struct BusStop
    {
        StopDirection Direction { get; set; }
        BasicGeoposition Position { get; set; }
        string ID { get; set; }
        string FriendlyName { get; set; }
    }
}
