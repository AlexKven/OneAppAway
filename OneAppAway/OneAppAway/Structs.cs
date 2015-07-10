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
        public string Name { get; set; }
        public string Code { get; set; }
        public int LocationType { get; set; }
        public string[] RouteIds { get; set; }

        public static bool operator ==(BusStop lhs, BusStop rhs)
        {
            return lhs.ID == rhs.ID;
        }

        public static bool operator !=(BusStop lhs, BusStop rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is BusStop)) return false;
            return this == (BusStop)obj;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }

    public struct BusArrival
    {
        public string RouteID { get; set; }

        public string TripID { get; set; }

        public string StopID { get; set; }

        public string RouteName { get; set; }

        //public string RouteLongName { get; set; }

        public DateTime ScheduledArrivalTime { get; set; }

        public DateTime? PredictedArrivalTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public string Destination { get; set; }

        public string Timeliness
        {
            get
            {
                if (PredictedArrivalTime == null) return "Unknown";
                int late = (int)(PredictedArrivalTime.Value - ScheduledArrivalTime).TotalMinutes;
                if (late == 0)
                    return "On Time";
                else if (late > 0)
                    return late.ToString() + "m Late";
                else
                    return (-late).ToString() + "m Early";
            }
        }
    }
}
