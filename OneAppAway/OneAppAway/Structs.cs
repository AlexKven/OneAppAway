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
        public string[] Routes { get; set; }

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
        public string Route { get; set; }

        public string Trip { get; set; }

        public string Stop { get; set; }

        public string RouteName { get; set; }

        //public string RouteLongName { get; set; }

        public DateTime ScheduledArrivalTime { get; set; }

        public DateTime? PredictedArrivalTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public string Destination { get; set; }

        public int MinutesLate
        {
            get
            {
                return (int)(PredictedArrivalTime.Value - ScheduledArrivalTime).TotalMinutes;
            }
        }

        public string TimelinessDescription
        {
            get
            {
                if (PredictedArrivalTime == null) return "Unknown";
                int late = MinutesLate;
                if (late == 0)
                    return "On Time";
                else if (late > 0)
                    return late.ToString() + "m Late";
                else
                    return (-late).ToString() + "m Early";
            }
        }

        public static bool operator ==(BusArrival lhs, BusArrival rhs)
        {
            return lhs.Trip == rhs.Trip && lhs.Stop == rhs.Stop;
        }

        public static bool operator !=(BusArrival lhs, BusArrival rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is BusArrival)) return false;
            return this == (BusArrival)obj;
        }

        public override int GetHashCode()
        {
            return (Trip + Stop).GetHashCode();
        }
    }

    public struct ScheduledArrival
    {
        public string Route { get; set; }

        public string Trip { get; set; }

        public string Stop { get; set; }

        public string RouteName { get; set; }

        public DateTime ScheduledArrivalTime { get; set; }

        public string Destination { get; set; }

        public static bool operator ==(ScheduledArrival lhs, ScheduledArrival rhs)
        {
            return lhs.Trip == rhs.Trip && lhs.Stop == rhs.Stop;
        }

        public static bool operator !=(ScheduledArrival lhs, ScheduledArrival rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ScheduledArrival)) return false;
            return this == (ScheduledArrival)obj;
        }

        public override int GetHashCode()
        {
            return (Trip + Stop).GetHashCode();
        }
    }

    public struct BusRoute
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Agency { get; set; }
    }

    public struct TransitAgency
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public string Url { get; set; }
    }

    public enum BandwidthOptions { Normal, Low, None, Auto }
}
