using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RouteViewPage : NavigationFriendlyPage
    {
        public RouteViewPage()
        {
            this.InitializeComponent();
            MainMap.MapServiceToken = Keys.BingMapKey;
        }

        private BusRoute Route;
        private BusTrip[] Trips;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string)
            {
                Route = await Data.GetRoute(e.Parameter.ToString());
                RouteNameBlock.Text = (Route.Name.All(chr => char.IsNumber(chr)) ? "Route " : "") + Route.Name;
                Trips = await ApiLayer.GetTripsForRoute(Route.ID);
                foreach (var trip in Trips)
                {
                    string shape = await ApiLayer.GetShape(trip.Shape);
                    MapPolyline line = new MapPolyline() { Path = new Windows.Devices.Geolocation.Geopath(HelperFunctions.DecodeShape(shape)), StrokeColor = Color.FromArgb(128,0,0,0), StrokeThickness = 4 };
                    MainMap.MapElements.Add(line);
                }
            }
        }
    }
}
