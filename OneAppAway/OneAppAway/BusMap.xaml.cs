using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private const double ZOOMLEVEL_SIZE_CUTOFF = 17;

        private List<Image> BusStopIcons = new List<Image>();
        private double PreviousZoomLevel;
        #endregion

        public BusMap()
        {
            this.InitializeComponent();
            _ShownStops.CollectionChanged += _ShownStops_CollectionChanged;
        }

        private ObservableCollection<BusStop> _ShownStops = new ObservableCollection<BusStop>();

        public ICollection<BusStop> ShownStops
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
                Geopoint result;
                MainMap.GetLocationFromOffset(new Point(0, 0), out result);
                return result.Position;
            }
        }

        public BasicGeoposition BottomRight
        {
            get
            {
                Geopoint result;
                MainMap.GetLocationFromOffset(new Point(ActualWidth - 1, ActualHeight - 1), out result);
                return result.Position;
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
            Image img = new Image();
            img.Tag = stop;
            string size = ZoomLevel < ZOOMLEVEL_SIZE_CUTOFF ? "20" : "40";
            img.Source = new BitmapImage(new Uri(stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + stop.Direction.ToString() + size + ".png"));
            MapControl.SetLocation(img, new Geopoint(stop.Position));
            MapControl.SetNormalizedAnchorPoint(img, new Point(0.5, 0.5));
            MainMap.Children.Add(img);
            BusStopIcons.Add(img);
        }

        public void UnOverlapIcons()
        {
            double latPP = LatitudePerPixel;
            double lonPP = LongitudePerPixel;
            double iconSize = ZoomLevel < ZOOMLEVEL_SIZE_CUTOFF ? 24 : 48;
            Func<Image, BasicGeoposition, Rect> getBounds = delegate (Image img, BasicGeoposition loc)
            {
                Point center;
                MainMap.GetOffsetFromLocation(new Geopoint(loc), out center);
                return new Rect(new Point(center.X - iconSize / 2, center.Y - iconSize / 2), new Size(iconSize, iconSize));
            };
            Action<Image, Image, StopDirection, StopDirection, BasicGeoposition, BasicGeoposition> deIntersect = delegate (Image img1, Image img2, StopDirection dir1, StopDirection dir2, BasicGeoposition loc1, BasicGeoposition loc2)
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
                        Image top;
                        Image bottom;
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
                        Image left;
                        Image right;
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
            List<Image> closestIcons = new List<Image>();
            foreach (Image img in BusStopIcons)
            {
                BusStop stop = (BusStop)img.Tag;
                double closestDist = double.PositiveInfinity;
                Image closestImg = null;
                foreach (Image otherImg in BusStopIcons)
                {
                    if (otherImg == img) continue;
                    BusStop otherStop = (BusStop)otherImg.Tag;
                    double distEW = Math.Abs(stop.Position.Longitude - otherStop.Position.Longitude) / lonPP;
                    double distNS = Math.Abs(stop.Position.Latitude - otherStop.Position.Latitude) / lonPP;
                    double dist = Math.Sqrt(distEW * distEW + distNS * distNS);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestImg = otherImg;
                    }
                }
                closestIcons.Add(closestImg);
            }
            List<Image> deIntersectedImages = new List<Image>();
            for (int i = 0; i < BusStopIcons.Count; i++)
            {
                Image img1 = BusStopIcons[i];
                //if (deIntersectedImages.Contains(img1)) continue;
                Image img2 = closestIcons[i];
                deIntersectedImages.Add(img2);
                if (img1 == null || img2 == null) continue;
                deIntersect(img1, img2, ((BusStop)img1.Tag).Direction, ((BusStop)img2.Tag).Direction, ((BusStop)img1.Tag).Position, ((BusStop)img2.Tag).Position);
            }
        }

        

        private void RefreshIconSizes()
        {
            foreach (Image img in BusStopIcons)
            {
                BusStop stop = (BusStop)img.Tag;
                string size = ZoomLevel < ZOOMLEVEL_SIZE_CUTOFF ? "20" : "40";
                img.Source = new BitmapImage(new Uri(stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + stop.Direction.ToString() + size + ".png"));
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
        }

        private void MainMap_ZoomLevelChanged(MapControl sender, object args)
        {
            if ((ZoomLevel < ZOOMLEVEL_SIZE_CUTOFF) != (PreviousZoomLevel < ZOOMLEVEL_SIZE_CUTOFF))
                RefreshIconSizes();
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




        private async void CenterOnMyHouse()
        {
            var res = await Windows.Services.Maps.MapLocationFinder.FindLocationsAsync("30143 12th Ave SW, Federal Way, WA", new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition() { Latitude = 47.3, Longitude = 122.3 }));
            //MapControl.SetLocation(FortyTwo, res.Locations[0].Point);


            MapIcon MapIcon1 = new MapIcon();
            MapIcon1.Location = res.Locations[0].Point;
            MapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            MapIcon1.Title = "4 Routes";
            MainMap.MapElements.Add(MapIcon1);

        }
    }
}
