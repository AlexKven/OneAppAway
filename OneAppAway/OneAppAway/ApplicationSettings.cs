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
        public static readonly DependencyProperty BandwidthSettingProperty = DependencyProperty.Register("BandwidthSetting", typeof(BandwidthOptions), typeof(ApplicationSettings), new PropertyMetadata(BandwidthOptions.Auto, OnBandwidthSettingChanged));

        public BandwidthOptions BandwidthSetting
        {
            get { return (BandwidthOptions)GetValue(BandwidthSettingProperty); }
            set { SetValue(BandwidthSettingProperty, value); }
        }

        private static void OnBandwidthSettingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BandwidthSettingStatic = (BandwidthOptions)e.NewValue;
        }

        public static BandwidthOptions BandwidthSettingStatic
        {
            get { return (BandwidthOptions)SettingsManager.GetSetting<int>("BandwidthOptions", false); }
            set { SettingsManager.SetSetting<int>("BandwidthOptions", false, (int)(value)); }
        }
    }
}
