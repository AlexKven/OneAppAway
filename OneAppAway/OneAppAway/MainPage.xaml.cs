using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RouteViewPage));
        }

        private async void CenterOnCurrentLocation()
        {
            var loc = new Geolocator();
            try
            {
                loc.DesiredAccuracy = PositionAccuracy.High;
                Geoposition pos = await loc.GetGeopositionAsync();
                MainBusMap.CenterOnLocation(pos.Coordinate.Point.Position, 17);
            }
            catch (Exception) { }
        }

        private void MainBusMap_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "ZoomLevel")
            //    MainBusMap.UnOverlapIcons();
            //Debug.WriteLine(MainBusMap.LatitudePerPixel / MainBusMap.LongitudePerPixel);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().Title = ActualWidth.ToString("F0") + " x " + ActualHeight.ToString("F0");
            
        }
    }
}
