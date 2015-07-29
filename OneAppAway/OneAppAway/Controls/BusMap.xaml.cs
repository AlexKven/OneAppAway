using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class BusMap : UserControl, INotifyPropertyChanged
    {
        #region Fields
        public const double ZOOMLEVEL_SIZE_CUTOFF = 16;
        public const double ZOOMLEVEL_VISIBILITY_CUTOFF = 14;
        private const double LAT_OVER_LON = .67706;

        private List<MapIcon> BusStopIcons = new List<MapIcon>();
        private Dictionary<MapIcon, BusStop> Stops = new Dictionary<MapIcon, BusStop>();
        private double PreviousZoomLevel;
        private long LastMove;
        #endregion

        public BusMap()
        {
            this.InitializeComponent();
            _ShownStops.CollectionChanged += _ShownStops_CollectionChanged;
        }

        private ObservableCollection<BusStop> _ShownStops = new ObservableCollection<BusStop>();

        private ICollection<BusStop> ShownStops
        {
            get
            {
                return _ShownStops;
            }
        }

        public BasicGeoposition Center
        {
            get
            {
                return MainMap.Center.Position;
            }
            set
            {
                MainMap.Center = new Geopoint(value);
                OnPropertyChanged("Center", "TopLeft", "BottomRight");
            }
        }

        public BasicGeoposition TopLeft
        {
            get
            {
                try
                {
                    Geopoint result;
                    MainMap.GetLocationFromOffset(new Point(0, 0), out result);
                    return result.Position;
                }
                catch (ArgumentException)
                {
                    return new BasicGeoposition();
                }
            }
        }

        public BasicGeoposition BottomRight
        {
            get
            {
                try
                {
                    Geopoint result;
                    MainMap.GetLocationFromOffset(new Point(ActualWidth - 1, ActualHeight - 1), out result);
                    return result.Position;
                }
                catch (ArgumentException)
                {
                    return new BasicGeoposition();
                }
            }
        }

        public double LatitudePerPixel
        {
            get
            {
                return (TopLeft.Latitude - BottomRight.Latitude) / ActualHeight;
            }
        }

        public double LongitudePerPixel
        {
            get
            {
                return (BottomRight.Longitude - TopLeft.Longitude) / ActualWidth;
            }
        }

        public double ZoomLevel
        {
            get
            {
                return MainMap.ZoomLevel;
            }
        }

        #region Methods
        private void AddStopToMap(BusStop stop)
        {
            MapIcon mico = new MapIcon();
            mico.Location = new Geopoint(stop.Position);
            mico.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
            string size = ZoomLevel < ZOOMLEVEL_SIZE_CUTOFF ? "20" : "40";
            bool visibility = ZoomLevel >= ZOOMLEVEL_VISIBILITY_CUTOFF;
            mico.Image = RandomAccessStreamReference.CreateFromUri(new Uri(stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + stop.Direction.ToString() + size + ".png"));
            mico.NormalizedAnchorPoint = new Point(0.5, 0.5);
            mico.Visible = visibility;
            MainMap.MapElements.Add(mico);
            Stops[mico] = stop;
            BusStopIcons.Add(mico);
        }

        private void RemoveStopFromMap(BusStop stop)
        {
            MapIcon mico = Stops.FirstOrDefault(kvp => kvp.Value == stop).Key;
            Stops.Remove(mico);
            if (mico == null) return;
            MainMap.MapElements.Remove(mico);
        }

        private void RefreshIconSizes()
        {
            foreach (MapIcon img in BusStopIcons)
            {
                BusStop stop = Stops[img];
                string size = ZoomLevel < ZOOMLEVEL_SIZE_CUTOFF ? "20" : "40";
                img.Image = RandomAccessStreamReference.CreateFromUri(new Uri(stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + stop.Direction.ToString() + size + ".png"));
                //img.Source = new BitmapMapIcon(new Uri(stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + stop.Direction.ToString() + size + ".png"));
            }
        }

        private void RefreshIconVisibilities()
        {
            bool visibility = ZoomLevel >= ZOOMLEVEL_VISIBILITY_CUTOFF;
            foreach (MapIcon img in BusStopIcons)
            {
                BusStop stop = Stops[img];
                img.Visible = visibility;
                //img.Source = new BitmapMapIcon(new Uri(stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + stop.Direction.ToString() + size + ".png"));
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (string propertyName in propertyNames)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler<StopClickedEventArgs> StopsClicked;

        private void OnStopsClicked(BusStop[] stops, BasicGeoposition location)
        {
            if (StopsClicked != null) StopsClicked(this, new StopClickedEventArgs(location, stops));
        }
        #endregion

        #region Event Handlers
        private void _ShownStops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (BusStop item in e.NewItems)
                {
                    AddStopToMap(item);
                }
            }
            else if (e.OldItems != null)
            {
                foreach (BusStop item in e.OldItems)
                {
                    RemoveStopFromMap(item);
                }
            }
        }

        private void MainMap_ZoomLevelChanged(MapControl sender, object args)
        {
            if ((ZoomLevel < ZOOMLEVEL_SIZE_CUTOFF) != (PreviousZoomLevel < ZOOMLEVEL_SIZE_CUTOFF))
                RefreshIconSizes();
            if ((ZoomLevel < ZOOMLEVEL_VISIBILITY_CUTOFF) != (PreviousZoomLevel < ZOOMLEVEL_VISIBILITY_CUTOFF))
                RefreshIconVisibilities();
            OnPropertyChanged("ZoomLevel", "TopLeft", "BottomRight");
            PreviousZoomLevel = ZoomLevel;
        }

        private void MainMap_CenterChanged(MapControl sender, object args)
        {
            OnPropertyChanged("Center", "TopLeft", "BottomRight");
        }

        private void MainMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnPropertyChanged("Center", "TopLeft", "BottomRight");
        }
        #endregion

        public async void CenterOnLocation(BasicGeoposition location, double zoomLevel)
        {
            await MainMap.TrySetViewAsync(new Geopoint(location), zoomLevel, null, null, MapAnimationKind.None);
        }

        private async void MainMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            Geopoint position;
            MainMap.GetLocationFromOffset(args.Position, out position);
            BasicGeoposition newCenter = position.Position;
            newCenter.Latitude = position.Position.Latitude + (TopLeft.Latitude - BottomRight.Latitude) / 2 - 50 * LatitudePerPixel;
            double halfLatSpan = (TopLeft.Latitude - BottomRight.Latitude) / 2.5;
            double halfLonSpan = (BottomRight.Longitude - TopLeft.Longitude) / 2.5;
            //await MainMap.TrySetViewBoundsAsync(new GeoboundingBox(TopLeft, BottomRight), new Thickness(0), MapAnimationKind.Linear);
            await MainMap.TrySetViewBoundsAsync(new GeoboundingBox(new BasicGeoposition() { Latitude = newCenter.Latitude + halfLatSpan, Longitude = newCenter.Longitude - halfLonSpan },
                new BasicGeoposition() { Latitude = newCenter.Latitude - halfLatSpan, Longitude = newCenter.Longitude + halfLonSpan }), null, MapAnimationKind.Linear);

            BusStop[] stops = new BusStop[args.MapElements.Count];
            for (int i = 0; i < stops.Length; i++)
            {
                stops[i] = Stops[(MapIcon)args.MapElements[i]];
            }
            OnStopsClicked(stops, args.Location.Position);
        }
    }

    public class StopClickedEventArgs : EventArgs
    {
        public StopClickedEventArgs(BasicGeoposition center, params BusStop[] stops)
        {
            Center = center;
            Stops = stops;
        }

        public BasicGeoposition Center { get; private set; }

        public BusStop[] Stops { get; private set; }
    }
}
