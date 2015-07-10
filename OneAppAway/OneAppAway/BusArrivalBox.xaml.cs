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
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class BusArrivalBox : UserControl
    {
        public BusArrivalBox()
        {
            this.InitializeComponent();
        }

        private BusArrival _Arrival;

        public BusArrival Arrival
        {
            get { return _Arrival; }
            set
            {
                _Arrival = value;

                RouteNumberBlock.Text = value.RouteName;
                switch (value.RouteName.Length)
                {
                    case 1:
                        RouteNumberBlock.FontSize = 27;
                        break;
                    case 2:
                        RouteNumberBlock.FontSize = 24;
                        break;
                    case 3:
                        RouteNumberBlock.FontSize = 20;
                        break;
                    case 4:
                        RouteNumberBlock.FontSize = 18;
                        break;
                    case 5:
                        RouteNumberBlock.FontSize = 17;
                        break;
                    case 6:
                        RouteNumberBlock.FontSize = 14;
                        break;
                    default:
                        RouteNumberBlock.FontSize = 13;
                        break;
                }
                ScheduledTimeBlock.Text = "(sched. " + value.ScheduledArrivalTime.ToString("h:mm") + ")";
                PredictedTimeBlock.Text = value.PredictedArrivalTime == null ? "Unknown" : (value.PredictedArrivalTime.Value.ToString("h:mm") + ", " + value.Timeliness);
                MinutesAwayBlock.Text = value.PredictedArrivalTime == null ? (value.ScheduledArrivalTime - DateTime.Now).TotalMinutes.ToString("F0") : (value.PredictedArrivalTime.Value - DateTime.Now).TotalMinutes.ToString("F0");
                MinutesAwayBlock.Foreground = value.PredictedArrivalTime == null ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.LightGreen);
                DestinationBlock.Text = value.Destination;
            }
        }
    }
}
