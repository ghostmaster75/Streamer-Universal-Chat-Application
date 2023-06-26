using Windows.Storage;

namespace Streamer_Universal_Chat_Application.Common
{
    public class AppSettings
    {
        private ApplicationDataContainer localSettings;

        public AppSettings()
        {
            localSettings = ApplicationData.Current.LocalSettings;
        }

        public void SaveSetting(string key, string value)
        {
            localSettings.Values[key] = value;
        }

        public string LoadSetting(string key)
        {
            if (localSettings.Values.ContainsKey(key))
            {
                return (string)localSettings.Values[key];
            }

            return null;
        }
    }

}
