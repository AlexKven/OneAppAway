using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OneAppAway
{
    public static class SettingsManager
    {
        public static T GetSetting<T>(string settingName, bool roaming)
        {
            ApplicationDataContainer curContainer = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            object result = curContainer.Values[settingName];
            if (result == null) return default(T);
            return (T)result;
        }

        public static void SetSetting<T>(string settingName, bool roaming, T value)
        {
            ApplicationDataContainer curContainer = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            if (!curContainer.Values.ContainsKey(settingName))
                curContainer.Values.Add(settingName, value);
            else
                curContainer.Values[settingName] = value;
        }
    }
}
