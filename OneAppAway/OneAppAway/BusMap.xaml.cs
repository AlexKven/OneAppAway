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
        private List<GeoboundingBox> BoundsCovered = new List<GeoboundingBox>();
        private Dictionary<MapIcon, BusStop> Stops = new Dictionary<MapIcon, BusStop>();
        private List<Tuple<MapIcon, MapIcon, double>> CloseIconPairs = new List<Tuple<MapIcon, MapIcon, double>>();
        private double PreviousZoomLevel;
        private long LastMove;

        private Task<BusStop[]> GetStopsTask;
        private CancellationTokenSource GetStopsCancellationTokenSource;
        #endregion

        public BusMap()
        {
            this.InitializeComponent();
            _ShownStops.CollectionChanged += _ShownStops_CollectionChanged;
            this.PropertyChanged += BusMap_PropertyChanged;
        }

        private void BusMap_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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

        public void CalculateCloseIconPairs()
        {
            foreach (MapIcon img in BusStopIcons)
            {
                BusStop stop = Stops[img];
                double closestDist = double.PositiveInfinity;
                MapIcon closestImg = null;
                foreach (MapIcon otherImg in BusStopIcons)
                {
                    if (otherImg == img) continue;
                    BusStop otherStop = Stops[otherImg];
                    double distEW = Math.Abs(stop.Position.Longitude - otherStop.Position.Longitude) * LAT_OVER_LON;
                    double distNS = Math.Abs(stop.Position.Latitude - otherStop.Position.Latitude);
                    double dist = Math.Sqrt(distEW * distEW + distNS * distNS);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestImg = otherImg;
                    }
                }
                CloseIconPairs.Add(new Tuple<MapIcon, MapIcon, double>(img, closestImg, closestDist));
            }
            foreach (MapIcon img in BusStopIcons)
            {
                var pairs = CloseIconPairs.Where(tup => Stops[tup.Item1] == Stops[img] || Stops[tup.Item2] == Stops[img]).ToList();
                if (pairs.Count <= 1) continue;
                pairs.Remove(pairs.First(tup0 => tup0.Item3 == pairs.Min(tup => tup.Item3)));
                foreach (var item in pairs)
                    CloseIconPairs.Remove(item);
            }
        }

        public void UnOverlapIcons()
        {
            double latPP = LatitudePerPixel;
            double lonPP = LongitudePerPixel;
            double iconSize = ZoomLevel < ZOOMLEVEL_SIZE_CUTOFF ? 24 : 48;
            Func<MapIcon, BasicGeoposition, Rect> getBounds = delegate (MapIcon img, BasicGeoposition loc)
            {
                Point center;
                MainMap.GetOffsetFromLocation(new Geopoint(loc), out center);
                return new Rect(new Point(center.X - iconSize / 2, center.Y - iconSize / 2), new Size(iconSize, iconSize));
            };
            Action<MapIcon, MapIcon, StopDirection, StopDirection, BasicGeoposition, BasicGeoposition> deIntersect = delegate (MapIcon img1, MapIcon img2, StopDirection dir1, StopDirection dir2, BasicGeoposition loc1, BasicGeoposition loc2)
            {
                Rect intersect = getBounds(img1, loc1);
                intersect.Intersect(getBounds(img2, loc2));
                if (intersect.Width > 0 && intersect.Height > 0)
                {
                    bool lengthen;
                    if ((dir1 == StopDirection.E || dir1 == StopDirection.W) && (dir2 == StopDirection.E || dir2 == StopDirection.W) && (dir1 != dir2))
                        lengthen = true;
                    else if ((dir1 == StopDirection.N || dir1 == StopDirection.S) && (dir2 == StopDirection.N || dir2 == StopDirection.S) && (dir1 != dir2))
                        lengthen = false;
                    else
                        lengthen = intersect.Width > intersect.Height;
                    if (lengthen)
                    {
                        MapIcon top;
                        MapIcon bottom;
                        BasicGeoposition lTop;
                        BasicGeoposition lBottom;
                        if (loc1.Latitude > loc2.Latitude)
                        {
                            top = img1;
                            lTop = loc1;
                            bottom = img2;
                            lBottom = loc2;
                        }
                        else
                        {
                            top = img2;
                            lTop = loc2;
                            bottom = img1;
                            lBottom = loc1;
                        }
                        double deltaLat = intersect.Height * latPP / 2;
                        MapControl.SetLocation(top, new Geopoint(new BasicGeoposition() { Latitude = lTop.Latitude + deltaLat, Longitude = lTop.Longitude }));
                        MapControl.SetLocation(bottom, new Geopoint(new BasicGeoposition() { Latitude = lBottom.Latitude - deltaLat, Longitude = lBottom.Longitude }));
                    }
                    else
                    {
                        MapIcon left;
                        MapIcon right;
                        BasicGeoposition lLeft;
                        BasicGeoposition lRight;
                        if (loc1.Longitude > loc2.Longitude)
                        {
                            right = img1;
                            lRight = loc1;
                            left = img2;
                            lLeft = loc2;
                        }
                        else
                        {
                            right = img2;
                            lRight = loc2;
                            left = img1;
                            lLeft = loc1;
                        }
                        double deltaLon = intersect.Width * lonPP / 2;
                        MapControl.SetLocation(left, new Geopoint(new BasicGeoposition() { Longitude = lLeft.Longitude - deltaLon, Latitude = lLeft.Latitude }));
                        MapControl.SetLocation(right, new Geopoint(new BasicGeoposition() { Longitude = lRight.Longitude + deltaLon, Latitude = lRight.Latitude }));
                    }
                }
            };
            //List<MapIcon> closestIcons = new List<MapIcon>();
            //foreach (MapIcon img in BusStopIcons)
            //{
            //    BusStop stop = (BusStop)img.Tag;
            //    double closestDist = double.PositiveInfinity;
            //    MapIcon closestImg = null;
            //    foreach (MapIcon otherImg in BusStopIcons)
            //    {
            //        if (otherImg == img) continue;
            //        BusStop otherStop = (BusStop)otherImg.Tag;
            //        double distEW = Math.Abs(stop.Position.Longitude - otherStop.Position.Longitude) / lonPP;
            //        double distNS = Math.Abs(stop.Position.Latitude - otherStop.Position.Latitude) / lonPP;
            //        double dist = Math.Sqrt(distEW * distEW + distNS * distNS);
            //        if (dist < closestDist)
            //        {
            //            closestDist = dist;
            //            closestImg = otherImg;
            //        }
            //    }
            //    closestIcons.Add(closestImg);
            //}
            //List<MapIcon> deIntersectedMapIcons = new List<MapIcon>();
            //for (int i = 0; i < BusStopIcons.Count; i++)
            //{
            //    MapIcon img1 = BusStopIcons[i];
            //    //if (deIntersectedMapIcons.Contains(img1)) continue;
            //    MapIcon img2 = closestIcons[i];
            //    deIntersectedMapIcons.Add(img2);
            //    if (img1 == null || img2 == null) continue;
            //    deIntersect(img1, img2, ((BusStop)img1.Tag).Direction, ((BusStop)img2.Tag).Direction, ((BusStop)img1.Tag).Position, ((BusStop)img2.Tag).Position);
            //}
            foreach (var item in CloseIconPairs)
            {
                deIntersect(item.Item1, item.Item2, Stops[item.Item1].Direction, Stops[item.Item2].Direction, Stops[item.Item1].Position, Stops[item.Item2].Position);
            }
        }

        private void AddStopsInBounds(GeoboundingBox bounds)
        {
            Action<BusStop[], GeoboundingBox> stopsLoadedCallback = delegate (BusStop[] stops, GeoboundingBox bnd) {
                foreach (var stop in stops)
                {
                    if (!ShownStops.Any(other => other == stop))
                        ShownStops.Add(stop);
                    BoundsCovered.Add(bnd);
                }
            };
            if (GetStopsTask != null && !GetStopsTask.IsCompleted)
            {
                GetStopsCancellationTokenSource.Cancel();
                //await GetStopsTask;
            }
            GetStopsCancellationTokenSource = new CancellationTokenSource();
            GetStopsTask = Data.GetStopsForArea(bounds, stopsLoadedCallback, GetStopsCancellationTokenSource.Token);
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

        private void OnStopsClicked(BusStop[] stops, BasicGeoposition location)
        {
            MapControl.SetLocation(StopArrivalBox, new Geopoint(location));
            StopArrivalBox.SetStops(stops);
            VisualStateManager.GoToState(this, "ArrivalBoxShown", true);
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
        #endregion

        #region Event Handlers
        private void _ShownStops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CloseIconPairs.Clear();
            if (e.NewItems != null)
            {
                foreach (BusStop item in e.NewItems)
                {
                    AddStopToMap(item);
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

        private async void MainMap_ActualCameraChanged(MapControl sender, MapActualCameraChangedEventArgs args)
        {
            GeoboundingBox bounds;
            try
            {
                bounds = new GeoboundingBox(TopLeft, BottomRight);
            }
            catch (Exception ex) { return; }
            long lm = DateTime.Now.Ticks;
            LastMove = lm;
            await Task.Delay(50);
            if (LastMove == lm)
            {
                if (MainMap.ZoomLevel < ZOOMLEVEL_VISIBILITY_CUTOFF) return;
                try
                {
                    AddStopsInBounds(bounds);
                }
                catch (TaskCanceledException) { }
            }
        }

        private void StopArrivalBox_CloseRequested(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "ArrivalBoxHidden", true);
        }
    }

    public class StopClickedEventArgs : EventArgs
    {
        public BusStop[] Stops { get; set; }
    }
}
