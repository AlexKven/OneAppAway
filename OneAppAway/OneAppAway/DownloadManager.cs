using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway
{
    public sealed class DownloadManager
    {
        #region Static
        private static List<DownloadManager> DownloadsInProgress = new List<DownloadManager>();

        public static async Task<DownloadManager> Create(RouteListing listing)
        {
            DownloadManager result = new DownloadManager();
            result.Listing = listing;
            result._Route = result.Listing.Route;
            result._StopsPending = new ObservableCollection<BusStop>((await ApiLayer.GetStopsForRoute(result.Listing.Route.ID)).Item1);
            result.Initialize();
            result._TotalStops = result.StopsPending.Count;
            DownloadsInProgress.Add(result);
            return result;
        }

        public static BusStop? DownloadNext()
        {
            if (DownloadsInProgress.Count == 0) return null;
            return DownloadsInProgress[0].StopsPending[0];
        }

        public static void StopDownloaded(BusStop stop)
        {
            foreach (var manager in DownloadsInProgress.ToArray())
            {
                if (manager.StopsPending.Contains(stop))
                    manager.StopsPending.Remove(stop);
            }
        }
        #endregion

        #region Instance
        private DownloadManager() { }

        private void Initialize()
        {
            _StopsPending.CollectionChanged += _StopsPending_CollectionChanged;
        }

        private void _StopsPending_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            double progress = 1.0 - (double)StopsPending.Count / (double)TotalStops;
            Listing.Progress = progress;
            Listing.ShowProgress = (progress != 1.0);
            if (StopsPending.Count == 0)
                DownloadsInProgress.Remove(this);
        }

        private RouteListing Listing;

        private ObservableCollection<BusStop> _StopsPending = new ObservableCollection<BusStop>();
        private BusRoute _Route;
        private int _TotalStops;

        public IList<BusStop> StopsPending
        {
            get { return _StopsPending; }
        }

        public BusRoute Route
        {
            get { return _Route; }
        }

        public int TotalStops
        {
            get { return _TotalStops; }
        }
        #endregion
    }

    public class RouteListing : DependencyObject
    {
        public RouteListing(BusRoute route)
        {
            Name = route.Name;
            Description = route.Description;
            Route = route;
        }

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(RouteListing), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ShowProgressProperty = DependencyProperty.Register("ShowProgress", typeof(bool), typeof(RouteListing), new PropertyMetadata(false));
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(RouteListing), new PropertyMetadata(false));

        public BusRoute Route { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
    }
}
