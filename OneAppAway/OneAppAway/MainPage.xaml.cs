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
            //CenterOnMyHouse();
            ApiTest();
        }

        private async void ApiTest()
        {
            HttpClient client = new HttpClient();
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.pugetsound.onebusaway.org/api/where/stops-for-location.xml?key=TEST&lat=47.653435&lon=-122.305641"));
            string respStr = await resp.Content.ReadAsStringAsync();
            respStr.ToString();
        }

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

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var loc = MainMap.Center;
            
            HttpClient client = new HttpClient();
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.pugetsound.onebusaway.org/api/where/stops-for-location.xml?key=TEST&radius=2000&lat=" + loc.Position.Latitude.ToString() + "&lon=" + loc.Position.Longitude.ToString()));

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
                MapIcon MapIcon1 = new MapIcon();
                MapIcon1.Location = new Geopoint(new BasicGeoposition() { Latitude = double.Parse(lat), Longitude = double.Parse(lon) });
                MapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
                MapIcon1.Title = "Bus Stop";
                MapIcon1.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Icons/BusDirectionN.png"));
                
                MainMap.MapElements.Add(MapIcon1);
            }
        }
    }
}
