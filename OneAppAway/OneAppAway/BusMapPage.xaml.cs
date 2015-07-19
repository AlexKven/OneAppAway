using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.Devices.Geolocation;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls.Maps;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using System.IO;
using System.Xml.Linq;
using System.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BusMapPage : NavigationFriendlyPage
    {
        public BusMapPage()
        {
            this.InitializeComponent();
            _ShownStops.CollectionChanged += _ShownStops_CollectionChanged;
            this.PropertyChanged += BusMap_PropertyChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //FileOpenPicker openPicker = new FileOpenPicker();
            //openPicker.FileTypeFilter.Add(".txt");
            //var res = await openPicker.PickSingleFileAsync();
            //var stream = await res.OpenAsync(Windows.Storage.FileAccessMode.Read);
            //byte[] buffer = new byte[stream.Size];
            //stream.AsStreamForRead().Read(buffer, 0, buffer.Length);
            //string str = new string(buffer.Select(bte => (char)bte).ToArray());
            StringReader reader = new StringReader(await ApiLayer.SendRequest("schedule-for-stop/1_431", new Dictionary<string, string>() {["date"] = "2015-07-19" }));
            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry").Element("stopRouteSchedules");
            StringBuilder builder = new StringBuilder();
            foreach (var el2 in el.Elements("stopRouteSchedule"))
            {
                string route = el2.Element("routeId")?.Value;
                builder.Append(route);
                builder.Append("(");
                foreach (var el3 in el2.Element("stopRouteDirectionSchedules").Elements("stopRouteDirectionSchedule"))
                {
                    string sign = el3.Element("tripHeadsign")?.Value;
                    builder.Append("\"");
                    builder.Append(sign);
                    builder.Append("\"(");
                    foreach (var el4 in el3.Element("scheduleStopTimes").Elements("scheduleStopTime"))
                    {
                        builder.Append("(");
                        string arrivalTime = el4.Element("arrivalTime")?.Value;
                        string tripId = el4.Element("tripId")?.Value;
                        DateTime time = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(long.Parse(arrivalTime))).ToLocalTime();
                        builder.Append(time.Hour * 60 + time.Minute);
                        builder.Append("|");
                        builder.Append(tripId);
                        builder.Append(")");
                    }
                    builder.Append(")");
                }
                builder.Append(")");
            }
            string str2 = builder.ToString();
            
            str2.ToString();
        }

        public void CenterOnCurrentLocation()
        {
            Data.ProgressivelyAcquireLocation(delegate (BasicGeoposition pos)
            {
#pragma warning disable CS4014
                MainMap.TrySetViewAsync(new Geopoint(pos), 17, null, null, MapAnimationKind.Linear);
#pragma warning restore CS4014
            });
        }

        private void MainBusMap_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "ZoomLevel")
            //    MainBusMap.UnOverlapIcons();
            //Debug.WriteLine(MainBusMap.LatitudePerPixel / MainBusMap.LongitudePerPixel);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

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
            set
            {
                MainMap.ZoomLevel = value;
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
            Action<BusStop[], GeoboundingBox> stopsLoadedCallback = delegate (BusStop[] stops, GeoboundingBox bnd)
            {
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
            GetStopsTask = Data.GetBusStopsForArea(bounds, stopsLoadedCallback, GetStopsCancellationTokenSource.Token);
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
            MapControl.SetLocation(StopArrivalGrid, new Geopoint(location));
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
            catch (Exception) { return; }
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

        private void WindowSizeVisualStates_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "NarrowState")
            {
                if (StopArrivalGrid.Children.Contains(StopArrivalBox))
                    StopArrivalGrid.Children.Remove(StopArrivalBox);
                if (!MainGrid.Children.Contains(StopArrivalBox))
                    MainGrid.Children.Add(StopArrivalBox);
                StopArrivalBox.Width = double.NaN;
                StopArrivalBox.Height = double.NaN;
            }
            else if (e.NewState.Name == "NormalState")
            {
                if (MainGrid.Children.Contains(StopArrivalBox))
                    MainGrid.Children.Remove(StopArrivalBox);
                if (!StopArrivalGrid.Children.Contains(StopArrivalBox))
                    StopArrivalGrid.Children.Add(StopArrivalBox);
                StopArrivalBox.Width = 350;
                StopArrivalBox.Height = 450;
            }
        }

        private void ArrivalBoxVisualStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            CanGoBack = (e.NewState.Name == "ArrivalBoxShown");
        }

        protected override void OnGoBack(ref bool handled)
        {
            handled = CanGoBack;
            VisualStateManager.GoToState(this, "ArrivalBoxHidden", true);
        }

        protected override void OnSaveState(Dictionary<string, object> state)
        {
            state.Add("Lat", Center.Latitude);
            state.Add("Lon", Center.Longitude);
            state.Add("Zoom", ZoomLevel);
            if (ArrivalBoxVisualStates?.CurrentState?.Name == "ArrivalBoxShown")
            {
                state.Add("Stops", StopArrivalBox.GetStops().Select(bs => bs.ID).ToArray());
                var boxPosition = MapControl.GetLocation(StopArrivalGrid);
                state.Add("StopsLat", boxPosition.Position.Latitude);
                state.Add("StopsLon", boxPosition.Position.Longitude);
            }
        }

        protected override async void OnLoadState(Dictionary<string, object> state, object navigationParameter)
        {
            if (state != null && state.ContainsKey("Lat") && state.ContainsKey("Lon") && state.ContainsKey("Zoom"))
            {
                Center = new BasicGeoposition() { Latitude = (double)state["Lat"], Longitude = (double)state["Lon"] };
                ZoomLevel = (double)state["Zoom"];
                if (state.ContainsKey("Stops") && state.ContainsKey("StopsLat") && state.ContainsKey("StopsLon"))
                {
                    var location = new BasicGeoposition() { Latitude = (double)state["StopsLat"], Longitude = (double)state["StopsLon"] };
                    var stopIds = (string[])state["Stops"];
                    BusStop[] stops = new BusStop[stopIds.Length];
                    for (int i = 0; i < stopIds.Length; i++)
                        stops[i] = await Data.GetBusStop(stopIds[i]);
                    OnStopsClicked(stops, location);
                    CanGoBack = true;
                }
            }
            else if (navigationParameter?.ToString() == "CurrentLocation")
            {
                CenterOnCurrentLocation();
            }
        }
    }
}
