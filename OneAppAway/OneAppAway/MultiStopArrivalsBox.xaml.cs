using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class MultiStopArrivalsBox : UserControl
    {
        private int StopsCount = 0;

        public MultiStopArrivalsBox()
        {
            this.InitializeComponent();
        }

        public void SetStops(params BusStop[] stops)
        {
            ItemsPanel.Children.Clear();
            foreach (BusStop stop in stops)
            {
                ItemsPanel.Children.Add(new StopArrivalsBox() { Stop = stop, Width = 280 });
            }
            StopsCount = stops.Length;
            if (_Caption == null)
                CaptionBox.Text = StopsCount.ToString() + (StopsCount == 1 ? " Stop" : " Stops");
        }

        private string _Caption = null;
        public string Caption
        {
            get { return _Caption; }
            set
            {
                _Caption = value;
                if (_Caption == null)
                    CaptionBox.Text = StopsCount.ToString() + (StopsCount == 1 ? " Stop" : " Stops");
                else
                    CaptionBox.Text = _Caption;
            }
        }
    }
}
