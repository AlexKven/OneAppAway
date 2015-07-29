using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway
{
    //public class RealtimeMediator : DependencyObject, INotifyPropertyChanged
    //{
    //    private static List<RealtimeMediator> RegisteredMediatiors = new List<RealtimeMediator>();
    //    private static Dictionary<string, RealtimeArrival[]> ArrivalsList = new Dictionary<string, RealtimeArrival[]>();
    //    private static DispatcherTimer RefreshTimer = new DispatcherTimer();
    //    private static DispatcherTimer NowTimer = new DispatcherTimer();

    //    static RealtimeMediator()
    //    {
    //        RefreshTimer.Interval = TimeSpan.FromSeconds(30);
    //        NowTimer.Interval = TimeSpan.FromSeconds(10);
    //        RefreshTimer.Tick += RefreshTimer_Tick;
    //        NowTimer.Tick += NowTimer_Tick;
    //        RefreshTimer.Start();
    //        NowTimer.Start();
    //    }

    //    public RealtimeMediator(string stopId, string tripId)
    //    {
    //        _StopId = stopId;
    //        _TripId = tripId;
    //    }

    //    #region Tick Handlers
    //    private static void NowTimer_Tick(object sender, object e)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private static async void RefreshTimer_Tick(object sender, object e)
    //    {
    //        await Refresh();
    //    }
    //    #endregion

    //    #region Methods
    //    public void Register()
    //    {
    //        if (!RegisteredMediatiors.Contains(this))
    //            RegisteredMediatiors.Add(this);
    //    }

    //    public void Deregister()
    //    {
    //        RegisteredMediatiors.Remove(this);
    //    }

    //    private static async Task RefreshList(string stopId)
    //    {
    //        ArrivalsList[stopId] = await ApiLayer.GetBusArrivals(stopId);
    //    }

    //    public static async Task Refresh(string stopId)
    //    {
    //        await RefreshList(stopId);
    //        var arrivalsList = ArrivalsList[stopId];
    //        foreach (var med in RegisteredMediatiors.Where(m => m.StopId == stopId))
    //        {
    //            med.MinutesUntilArrival = 
    //        }
    //    }

    //    public static async Task Refresh()
    //    {
    //        foreach (var stop in ArrivalsList.Keys)
    //            await Refresh(stop);
    //    }

    //    public static RealtimeArrival[] GetArrivals(string stop)
    //    {
    //        return ArrivalsList[stop];
    //    }
    //    #endregion

    //    #region Properties
    //    private string _MinutesUntilArrival = "";
    //    private string _ArrivalTimeReading = "";
    //    private string _ScheduledTimeReading = "";
    //    private object _Tag;
    //    private string _StopId;
    //    private string _TripId;

    //    public string MinutesUntilArrival
    //    {
    //        get { return _MinutesUntilArrival; }
    //        private set { _MinutesUntilArrival = value; }
    //    }

    //    public string ArrivalTimeReading
    //    {
    //        get { return _ArrivalTimeReading; }
    //        private set { _ArrivalTimeReading = value; }
    //    }

    //    public string ScheduledTimeReading
    //    {
    //        get { return _ScheduledTimeReading; }
    //        private set { _ScheduledTimeReading = value; }
    //    }

    //    public object Tag
    //    {
    //        get { return _Tag; }
    //        set
    //        {
    //            _Tag = value;
    //            OnPropertyChanged("Tag");
    //        }
    //    }

    //    public string StopId
    //    {
    //        get { return _StopId; }
    //    }

    //    public string TripId
    //    {
    //        get { return _TripId; }
    //    }
    //    #endregion

    //    #region Events
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    public static event EventHandler<StopRefreshedArgs> StopRefreshed;

    //    private void OnPropertyChanged(params string[] propertyNames)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            foreach (string propertyName in propertyNames)
    //                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }
    //    #endregion
    //}

    //public class StopRefreshedArgs : EventArgs
    //{
    //    public string StopId { get; private set; }

    //    public StopRefreshedArgs(string stopId)
    //    {
    //        StopId = stopId;
    //    }
    //}
}
