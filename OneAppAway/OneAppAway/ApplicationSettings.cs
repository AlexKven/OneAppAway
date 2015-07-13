using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway
{
    public class ApplicationSettings : DependencyObject
    {
        public static readonly DependencyProperty BandwidthSettingProperty = DependencyProperty.Register("BandwidthSetting", typeof(BandwidthOptions), typeof(ApplicationSettings), new PropertyMetadata(BandwidthOptions.Auto));

        public BandwidthOptions BandwidthSetting
        {
            get { return (BandwidthOptions)GetValue(BandwidthSettingProperty); }
            set { SetValue(BandwidthSettingProperty, value); }
        }
    }
}
