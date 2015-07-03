using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace OneAppAway
{
    public static class HelperFunctions
    {
        public static GeoboundingBox Intersect(this GeoboundingBox bound1, params GeoboundingBox[] boundn)
        {
            Rect result = bound1.ToRect();
            foreach (GeoboundingBox bound in boundn)
            {
                result.Intersect(bound.ToRect());
            }
            return result.ToGeoboundingBox();
        }

        public static Rect ToRect(this GeoboundingBox bounds)
        {
            return new Rect(new Point(bounds.NorthwestCorner.Longitude, bounds.SoutheastCorner.Latitude), new Point(bounds.SoutheastCorner.Longitude, bounds.NorthwestCorner.Latitude));
        }

        public static GeoboundingBox ToGeoboundingBox(this Rect bounds)
        {
            return new GeoboundingBox(new BasicGeoposition() { Latitude = bounds.Bottom, Longitude = bounds.Left }, new BasicGeoposition() { Latitude = bounds.Top, Longitude = bounds.Right });
        }
    }
}
