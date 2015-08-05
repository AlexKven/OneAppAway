using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static System.Math;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoutesPage : Page
    {
        public RoutesPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            foreach (var agency in await ApiLayer.GetTransitAgencies())
            {
                AgenciesListView.Items.Add(agency);
                AgenciesListView.SelectedIndex = 0;
            }
        }

        private async void AgenciesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainProgressRing.IsActive = true;
            var inOrder = (Func<BusRoute, BusRoute, bool>)delegate (BusRoute first, BusRoute second)
            {
                var splitName = (Func<string, Tuple<int, string>>)delegate (string name)
                {
                    string num = new string(name.TakeWhile(chr => char.IsNumber(chr)).ToArray());
                    if (num.Length == 0)
                        return new Tuple<int, string>(0, name);
                    else
                        return new Tuple<int, string>(int.Parse(num), name.Replace(num, ""));
                };
                var splitFirst = splitName(first.Name);
                var splitSecond = splitName(second.Name);
                if (splitFirst.Item1 < splitSecond.Item1)
                    return true;
                else if (splitFirst.Item1 > splitSecond.Item1)
                    return false;
                else
                    return string.Compare(splitFirst.Item2, splitSecond.Item2, StringComparison.CurrentCultureIgnoreCase) < 0;
            };
            MainList.Items.Clear();
            var agency = (TransitAgency)AgenciesListView.SelectedItem;
            SortedSet<BusRoute> routesList = new SortedSet<BusRoute>(Comparer<BusRoute>.Create(new Comparison<BusRoute>((rt1, rt2) => inOrder(rt1, rt2) ? -1 : 1)));
            foreach (var rte in await ApiLayer.GetBusRoutes(agency.Id))
            {
                routesList.Add(rte);
            }
            foreach (var rte in routesList)
                MainList.Items.Add(rte);
            MainProgressRing.IsActive = false;
        }

        private void Route_Clicked(object sender, EventArgs e)
        {
            Frame.Navigate(typeof(RouteViewPage), ((RouteListingControl)sender).Route.ID);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainList.Tag = ActualWidth / (Max((int)ActualWidth / 400, 1.0));
        }
    }
}
