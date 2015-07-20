using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StopViewPage : NavigationFriendlyPage
    {
        public StopViewPage()
        {
            this.InitializeComponent();
            RoutesToggle.IsChecked = true;
            ArrivalsToggle.IsChecked = true;
        }

        private double? lonPP;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null && e.Parameter is string)
            {
                SetPage((string)e.Parameter);
            }
        }

        private async void SetPage(string stopId)
        {
            Stop = await Data.GetBusStop(stopId);
            TitleBlock.Text = Stop.Name;
            Uri imageUri = new Uri(Stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase40.png" : "ms-appx:///Assets/Icons/BusDirection" + Stop.Direction.ToString() + "40.png");
            DirectionImage.Source = new BitmapImage(imageUri);
            MapIcon mico = new MapIcon();
            mico.Location = new Geopoint(Stop.Position);
            mico.Image = RandomAccessStreamReference.CreateFromUri(imageUri);
            mico.NormalizedAnchorPoint = new Point(0.5, 0.5); 
            MainMap.MapElements.Add(mico);
            MainMap.MapElements.Add(mico);
            MainMap.Center = new Geopoint(Stop.Position);
#pragma warning disable CS4014
            RefreshRoutes();
            RefreshArrivals();
            GetSchedule();
#pragma warning restore CS4014
            SetMapCenter();
        }

        private void SetMapCenter()
        {
            if (InnerGrid.ActualWidth == 0) return;
            if (lonPP == null)
            {
                Geopoint pointOutW;
                Geopoint pointOutE;
                MainMap.GetLocationFromOffset(new Point(0, 0), out pointOutW);
                MainMap.GetLocationFromOffset(new Point(InnerGrid.ActualWidth, 0), out pointOutE);
                lonPP = (pointOutE.Position.Longitude - pointOutW.Position.Longitude) / InnerGrid.ActualWidth;
            }
            MainMap.Center = new Geopoint(new BasicGeoposition() { Latitude = Stop.Position.Latitude, Longitude = Stop.Position.Longitude - lonPP.Value * (InnerGrid.ActualWidth - 100 - InnerGrid.ActualWidth / 2) });
        }

        private BusStop Stop;

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetInnerGridSize();
        }

        private void SetInnerGridSize()
        {
            InnerGrid.MinWidth = InnerGrid.ColumnDefinitions.Where(cd => cd.Width.IsStar).Count() * 300 + 200;
            if (MainGrid.ActualWidth > 0)
            {
                InnerGrid.Width = MainGrid.ActualWidth;
            }
        }

        private void SetColumns()
        {
            ArrivalsColumn.Width = ArrivalsToggle.IsChecked.Value ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            ScheduleColumn.Width = ScheduleToggle.IsChecked.Value ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            RoutesColumn.Width = RoutesToggle.IsChecked.Value ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            SetInnerGridSize();
        }

        private async Task RefreshArrivals()
        {
            ArrivalsProgressIndicator.IsActive = true;
            var arrivals = await ApiLayer.GetBusArrivals(Stop.ID);
            var removals = ArrivalsStackPanel.Children.Where(child => !arrivals.Contains(((BusArrivalBox)child).Arrival));
            foreach (var item in removals)
                ArrivalsStackPanel.Children.Remove(item);
            foreach (var item in arrivals)
            {
                if (ArrivalsStackPanel.Children.Any(child => ((BusArrivalBox)child).Arrival == item))
                    ((BusArrivalBox)ArrivalsStackPanel.Children.First(child => ((BusArrivalBox)child).Arrival == item)).Arrival = item;
                else
                    ArrivalsStackPanel.Children.Add(new BusArrivalBox() { Arrival = item });
            }
            LastRefreshBox.Text = "Last update: " + DateTime.Now.ToString("h:mm:ss");
            ArrivalsProgressIndicator.IsActive = false;
        }

        private async Task GetSchedule()
        {
            Schedule = new WeekSchedule();
            for (int i = 0; i < 7; i++)
            {
                DaySchedule daySched = new DaySchedule();
                daySched.LoadFromVerboseString(await ApiLayer.SendRequest("schedule-for-stop/" + Stop.ID, new Dictionary<string, string>() {["date"] = "2015-07-" + (13 + i).ToString() }));
                ServiceDay day = (ServiceDay)(int)Math.Pow(2, i);
                Schedule.AddServiceDay(day, daySched);
                if (i == 0)
                    Schedule.AddServiceDay(ServiceDay.ReducedWeekday, daySched);
            }
            foreach (var item in Schedule[ServiceDay.Weekdays])
            {
                ScheduledArrivalsPanel.Children.Add(new TextBlock() { Text = item.ScheduledArrivalTime.ToString("h:mm") + " to " + item.Destination, FontWeight = item.ScheduledArrivalTime.Hour >= 12 ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal });
            }
        }

        private WeekSchedule Schedule;

        private async Task RefreshRoutes()
        {
            RoutesProgressIndicator.IsActive = true;
            RoutesControl.Items.Clear();
            foreach (string rte in Stop.Routes)
            {
                BusRoute route = await Data.GetRoute(rte);
                RoutesControl.Items.Add(new { Name = route.Name, Description = route.Description, Agency = (await Data.GetTransitAgency(route.Agency)).Name });
            }
            RoutesProgressIndicator.IsActive = false;
        }

        private void InnerGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetMapCenter();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
#pragma warning disable CS4014
            RefreshArrivals();
            RefreshRoutes();
#pragma warning restore CS4014
        }

        private void AppBarToggleButton_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!(ArrivalsToggle.IsChecked.Value || ScheduleToggle.IsChecked.Value || RoutesToggle.IsChecked.Value))
                ArrivalsToggle.IsChecked = true;
            else
                SetColumns();
        }
    }
}
