using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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
        
        public static List<T> AllChildrenOfType<T>(DependencyObject parent) where T : DependencyObject
        {
            var _List = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var _Child = VisualTreeHelper.GetChild(parent, i);
                if (_Child is T)
                {
                    _List.Add(_Child as T);
                }
                _List.AddRange(AllChildrenOfType<T>(_Child));
            }
            return _List;
        }

        public static T FindControl<T>(DependencyObject parentContainer, string controlName) where T : DependencyObject
        {
            var childControls = AllChildrenOfType<T>(parentContainer);
            var control = childControls.OfType<Control>().Where(x => x.Name.Equals(controlName)).Cast<T>().First();
            return control;
        }
    }
}
