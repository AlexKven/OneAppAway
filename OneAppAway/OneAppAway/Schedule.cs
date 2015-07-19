using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OneAppAway
{
    [Flags]
    public enum ServiceDay : byte
    {
        None = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,
        ReducedWeekday = 128,
        Weekdays= 31,
        AllWeekdays = 159,
        Weekends = 96,
        All = 255
    }

    public class WeekSchedule
    {
        private List<ServiceDay> Days = new List<ServiceDay>();
        private List<DaySchedule> DaySchedules = new List<DaySchedule>();

        public WeekSchedule() { }

        public string Stop { get; set; }

        public void AddServiceDay(ServiceDay day, DaySchedule schedule)
        {
            for (int i = 0; i < Days.Count; i++)
            {
                if (DaySchedules[i].IsIdenticalTo(schedule))
                {
                    Days[i] |= day;
                    return;
                }
            }
            Days.Add(day);
            DaySchedules.Add(schedule);
        }

        public DaySchedule this[ServiceDay day]
        {
            get
            {
                for (int i = 0; i < Days.Count; i++)
                {
                    if (Days[i].HasFlag(day))
                        return DaySchedules[i];
                }
                return null;
            }
        }

        public ServiceDay[] DayGroups
        {
            get { return Days.ToArray(); }
        }
    }

    public class DaySchedule
    {
        private Tuple<string, string, Tuple<short, string>[]>[] Data;
        private string[] TripIds;

        public DaySchedule(){}

        public void LoadFromVerboseString(string str)
        {
            StringReader reader = new StringReader(str);
            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry").Element("stopRouteSchedules");

            List<Tuple<string, string, Tuple<short, string>[]>> data = new List<Tuple<string, string, Tuple<short, string>[]>>();
            foreach (var el2 in el.Elements("stopRouteSchedule"))
            {
                string route = el2.Element("routeId")?.Value;
                foreach (var el3 in el2.Element("stopRouteDirectionSchedules").Elements("stopRouteDirectionSchedule"))
                {
                    string sign = el3.Element("tripHeadsign")?.Value;
                    List<Tuple<short, string>> trips = new List<Tuple<short, string>>();
                    foreach (var el4 in el3.Element("scheduleStopTimes").Elements("scheduleStopTime"))
                    {
                        string arrivalTime = el4.Element("arrivalTime")?.Value;
                        string tripId = el4.Element("tripId")?.Value;
                        DateTime time = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(long.Parse(arrivalTime))).ToLocalTime();
                        trips.Add(new Tuple<short, string>((short)(time.Hour * 60 + time.Minute), tripId));
                    }
                    data.Add(new Tuple<string, string, Tuple<short, string>[]>(route, sign, trips.ToArray()));
                }
            }
            Data = data.ToArray();
        }

        public bool IsIdenticalTo(DaySchedule other)
        {
            List<string> ids = GetTripIds(true);
            List<string> otherIds = other.GetTripIds(false);
            int lastInd = ids.Count - 1;
            while (lastInd >= 0)
            {
                if (!otherIds.Remove(ids[lastInd]))
                    return false;
                ids.RemoveAt(lastInd);
                lastInd--;
            }
            return otherIds.Count == 0;
        }

        private List<string> GetTripIds(bool save)
        {
            if (TripIds == null)
            {
                List<string> result = new List<string>();
                foreach (var item0 in Data)
                {
                    foreach (var item1 in item0.Item3)
                    {
                        result.Add(item1.Item2);
                    }
                }
                if (save)
                    TripIds = result.ToArray();
                return result;
            }
            else
                return TripIds.ToList();
        }
    }
}
