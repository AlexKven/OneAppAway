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
            Func<Color, Color> darken = clr => Color.FromArgb(clr.A, (byte)(clr.R / 2), (byte)(clr.G / 2), (byte)(clr.B / 2));
            Func<Color, Color> lighten = clr => Color.FromArgb(clr.A, (byte)(128 + clr.R / 2), (byte)(128 + clr.G / 2), (byte)(1287 + clr.B / 2));
            this.InitializeComponent();
            Color accentColor = ((Color)App.Current.Resources["SystemColorControlAccentColor"]);
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Color.FromArgb(255, byte.Parse("05", System.Globalization.NumberStyles.HexNumber), byte.Parse("05", System.Globalization.NumberStyles.HexNumber), byte.Parse("05", System.Globalization.NumberStyles.HexNumber));
            titleBar.InactiveBackgroundColor = titleBar.BackgroundColor;
            //titleBar.BackgroundColor = darken(accentColor);
            titleBar.ForegroundColor = accentColor;
            titleBar.InactiveForegroundColor = darken(accentColor);
            titleBar.ButtonBackgroundColor = titleBar.BackgroundColor;
            titleBar.ButtonForegroundColor = titleBar.ForegroundColor;
            //titleBar.InactiveBackgroundColor = Color.FromArgb(255, byte.Parse("20", System.Globalization.NumberStyles.HexNumber), byte.Parse("20", System.Globalization.NumberStyles.HexNumber), byte.Parse("20", System.Globalization.NumberStyles.HexNumber));
            //titleBar.InactiveBackgroundColor = accentColor;
            //titleBar.InactiveForegroundColor = Colors.White;
            titleBar.ButtonInactiveBackgroundColor = titleBar.InactiveBackgroundColor;
            titleBar.ButtonInactiveForegroundColor = titleBar.InactiveForegroundColor;

            //ApplicationView.GetForCurrentView().Title = StopDirection.NE.ToString();
            //CenterOnMyHouse();
            //ApiTest();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            CenterOnCurrentLocation();
        }

        private async void ApiTest()
        {
            HttpClient client = new HttpClient();
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.pugetsound.onebusaway.org/api/where/stops-for-location.xml?key=TEST&lat=47.653435&lon=-122.305641"));
            string respStr = await resp.Content.ReadAsStringAsync();
            respStr.ToString();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await ApiLayer.GetBusArrivals("1_13283");
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

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void MainBusMap_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "ZoomLevel")
            //    MainBusMap.UnOverlapIcons();
            //Debug.WriteLine(MainBusMap.LatitudePerPixel / MainBusMap.LongitudePerPixel);
        }

        private void MainBusMap_StopClicked(object sender, StopClickedEventArgs e)
        {
            StopView.Children.Clear();
            foreach (var stop in e.Stops)
            {
                StopView.Children.Add(new TextBlock() { FontSize = 20, Text = stop.Name, Margin = new Thickness(10), TextWrapping = TextWrapping.Wrap });
            }
            StopInfoPanel.Visibility = Visibility.Visible;
            DoubleAnimation fadeIn = new DoubleAnimation();
            Storyboard.SetTarget(fadeIn, StopInfoPanel);
            Storyboard.SetTargetProperty(fadeIn, "Opacity");
            fadeIn.From = 0;
            fadeIn.To = 1;
            fadeIn.Duration = TimeSpan.FromMilliseconds(200);
            fadeIn.BeginTime = TimeSpan.FromSeconds(1);
            Storyboard sb = new Storyboard();
            sb.Children.Add(fadeIn);
            sb.Begin();
            
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().Title = ActualWidth.ToString("F0") + " x " + ActualHeight.ToString("F0");
        }
    }
}
