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
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Color.FromArgb(255, byte.Parse("88", System.Globalization.NumberStyles.HexNumber), byte.Parse("CC", System.Globalization.NumberStyles.HexNumber), byte.Parse("00", System.Globalization.NumberStyles.HexNumber));
            titleBar.ForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = titleBar.BackgroundColor;
            titleBar.ButtonForegroundColor = Colors.Green;
            //ApplicationView.GetForCurrentView().Title = StopDirection.NE.ToString();
            //CenterOnMyHouse();
            //ApiTest();
            
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
            var loc = MainBusMap.Center;
            double latSpan = Math.Abs(MainBusMap.BottomRight.Latitude - MainBusMap.TopLeft.Latitude);
            double lonSpan = Math.Abs(MainBusMap.BottomRight.Longitude - MainBusMap.TopLeft.Longitude);

            HttpClient client = new HttpClient();
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.pugetsound.onebusaway.org/api/where/stops-for-location.xml?key=TEST&lat=" + loc.Latitude.ToString() + "&lon=" + loc.Longitude.ToString() + "&latSpan=" + latSpan.ToString() + "&lonSpan=" + lonSpan.ToString()));

            var responseString = await resp.Content.ReadAsStringAsync();

            StringReader reader = new StringReader(responseString);
            XDocument xDoc = XDocument.Load(reader);

            //XElement el = (XElement)xDoc.Nodes().First(d => d.NodeType == XmlNodeType.Element && ((XElement)d).Name.LocalName == "response");
            XElement el = xDoc.Element("response");

            XElement el1 = el.Element("data");
            XElement el2 = el1.Element("list");

            foreach (XElement el3 in el2.Elements("stop"))
            {
                string lat = el3.Element("lat").Value;
                string lon = el3.Element("lon").Value;
                string direction = el3.Element("direction") == null ? null : el3.Element("direction").Value;
                BusStop stop = new BusStop() { Position = new BasicGeoposition() { Latitude = double.Parse(lat), Longitude = double.Parse(lon) }, Direction = direction == null ? StopDirection.Unspecified : (StopDirection)Enum.Parse(typeof(StopDirection), direction) };
                MainBusMap.ShownStops.Add(stop);
            }
            MainBusMap.UnOverlapIcons();
            Debug.WriteLine("Number of stops: " + MainBusMap.ShownStops.Count);
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void MainBusMap_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ZoomLevel")
                MainBusMap.UnOverlapIcons();
        }
    }
}
