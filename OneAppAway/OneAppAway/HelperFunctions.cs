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
using static OneAppAway.ServiceDay;

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

        public static string GetFriendlyName(this ServiceDay day) => GetFriendlyName(day, false);

        private static string GetFriendlyName(this ServiceDay day, bool rec)
        {
            if (day == All)
                return "Every Day";
            else if (day == (Weekdays | Weekends))
                return "Every Day*";
            else if (day.HasFlag(Saturday) && day.HasFlag(Sunday))
            {
                ServiceDay others = day & ~Saturday & ~Sunday;
                if (others == None)
                    return "Weekends";
                else
                    return "Weekends, " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Saturday))
            {
                ServiceDay others = day & ~Saturday;
                if (others == None)
                    return "Saturday";
                else
                    return "Sat, " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Sunday))
            {
                ServiceDay others = day & ~Sunday;
                if (others == None)
                    return "Sunday";
                else
                    return "Sun, " + others.GetFriendlyName(true);
            }
            else  if (day.HasFlag(Weekdays))
            {
                string result = "Weekdays";
                ServiceDay others = day & ~Weekdays;
                if (others.HasFlag(ReducedWeekday))
                    others = others & ~ReducedWeekday;
                else
                    result += "*";
                if (others == None)
                    return result;
                else
                    return result + ", " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Monday))
            {
                bool reduced = day.HasFlag(ReducedWeekday);
                ServiceDay others = day & ~Monday;
                if (reduced)
                    others = others & ~ReducedWeekday;
                if (others == None)
                    return rec ? reduced ? "Mon" : "Mon*" : reduced ? "Monday" : "Monday*";
                else
                    return (reduced ? "Mon" : "Mon*") + ", " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Tuesday))
            {
                ServiceDay others = day & ~Tuesday;
                if (others == None)
                    return rec ? "Tue" : "Tuesday";
                else
                    return "Tue, " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Wednesday))
            {
                ServiceDay others = day & ~Wednesday;
                if (others == None)
                    return rec ? "Wed" : "Wednesday";
                else
                    return "Wed, " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Thursday))
            {
                ServiceDay others = day & ~Thursday;
                if (others == None)
                    return rec ? "Thu" : "Thursday";
                else
                    return "Thu, " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Friday))
            {
                ServiceDay others = day & ~Friday;
                if (others == None)
                    return rec ? "Fri" : "Friday";
                else
                    return "Fri, " + others.GetFriendlyName(true);
            }
            else
            {
                return "None";
            }
        }
    }
}
