using Newtonsoft.Json.Linq;
using Streamer_Universal_Chat_Application.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace Streamer_Universal_Chat_Application
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        private Boolean twitchEnable;
        private String twitchUsername;
        private String twitchToken;
        private String twitchChannel;
        private Boolean tikTokEnable;
        private String tikTokUserNane;
        private AppBarButton saveButton;
        private AppBarButton undoButton;
        private TextBox textBoxTwitchUser;
        private PasswordBox passwordBoxTwitchToken;
        private TextBox textBoxTwitchChannel;
        private TextBox textBoxTikTokUserNane;
        private ToggleSwitch toogleSwitchTwitchEnable;
        private ToggleSwitch toogleSwitchTikTokEnable;
        private AppSettings settings = new AppSettings();

        public Settings()
        {
            this.InitializeComponent();
            this.LoadSettings();

            saveButton = (AppBarButton)FindName("SaveButton");
            undoButton = (AppBarButton)FindName("UndoButton");
            toogleSwitchTwitchEnable = (ToggleSwitch)FindName("TwitchEnable");
            textBoxTwitchUser = (TextBox)FindName("TwitchUserNane");
            passwordBoxTwitchToken = (PasswordBox)FindName("TwitchAccessToken");
            textBoxTwitchChannel = (TextBox)FindName("TwitchChannel");
            toogleSwitchTikTokEnable = (ToggleSwitch)FindName("TiktokEnable");
            textBoxTikTokUserNane = (TextBox)FindName("TikTokUserNane");

            toogleSwitchTikTokEnable.IsOn = tikTokEnable;
            toogleSwitchTwitchEnable.IsOn = twitchEnable;

            if (twitchUsername != null)
            {
                textBoxTwitchUser.Text = twitchUsername;
            }
            else
            {
                textBoxTwitchUser.Text = "";
            }

            if (twitchToken != null)
            {
                passwordBoxTwitchToken.Password = twitchToken;
            }
            else
            {
                passwordBoxTwitchToken.Password = "";
            }

            if (twitchChannel != null)
            {
                textBoxTwitchChannel.Text = twitchChannel;
            }
            else
            {
                textBoxTwitchChannel.Text = "";
            }

            if (tikTokUserNane != null)
            {
                textBoxTikTokUserNane.Text = tikTokUserNane;
            }
            else
            {
                textBoxTikTokUserNane.Text = "";
            }
        }

        private void LoadSettings()
        {
            bool value;
            bool success = bool.TryParse(settings.LoadSetting("TwitchEnable"), out value);
            if (success)
            {
                twitchEnable = value;
            } else { 
                twitchEnable = true; ;
            }
            

            twitchUsername = settings.LoadSetting("TwitchUsername");
            twitchToken = settings.LoadSetting("TwitchToken");
            twitchChannel = settings.LoadSetting("TwitchChannel");

            success = bool.TryParse(settings.LoadSetting("TiktokEnable"), out value);
            if (success)
            {
                tikTokEnable = value;
            }
            else
            {
                tikTokEnable = true; ;
            }

            tikTokUserNane = settings.LoadSetting("TikTokUserNane");

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            settings.SaveSetting("TwitchEnable", TwitchEnable.IsOn.ToString());
            settings.SaveSetting("TwitchUsername", textBoxTwitchUser.Text);
            settings.SaveSetting("TwitchToken", passwordBoxTwitchToken.Password);
            settings.SaveSetting("TwitchChannel", textBoxTwitchChannel.Text);
            settings.SaveSetting("TiktokEnable", TiktokEnable.IsOn.ToString());
            settings.SaveSetting("TikTokUserNane", textBoxTikTokUserNane.Text);
            this.LoadSettings();
            this.DisableButtons();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            toogleSwitchTwitchEnable.IsOn = twitchEnable;
            textBoxTwitchUser.Text = twitchUsername;
            passwordBoxTwitchToken.Password = twitchToken;
            textBoxTwitchChannel.Text = twitchChannel;
            toogleSwitchTikTokEnable.IsOn = tikTokEnable;
            textBoxTikTokUserNane.Text = tikTokUserNane;
            this.DisableButtons();
        }


        private void Settings_Changed(object sender, RoutedEventArgs e)
        {
            if (this.IsSettingsChanged())
            {
                this.EnableButtons();
            }
        }

        private void Enable_Click(object sender, RoutedEventArgs e)
        {
            var selectedFlyoutItem = sender as ToggleSwitch;
            settings.SaveSetting(selectedFlyoutItem.Name, selectedFlyoutItem.IsOn.ToString());
        }

        private void EnableButtons()
        {
            saveButton.IsEnabled = true;
            undoButton.IsEnabled = true;
        }
        private void DisableButtons()
        {
            saveButton.IsEnabled = false;
            undoButton.IsEnabled = false;
        }

        private Boolean IsSettingsChanged()
        {
            if (textBoxTwitchUser.Text != twitchUsername || passwordBoxTwitchToken.Password != twitchToken || textBoxTwitchChannel.Text != twitchChannel || textBoxTikTokUserNane.Text != tikTokUserNane || toogleSwitchTwitchEnable.IsOn != twitchEnable || toogleSwitchTikTokEnable.IsOn != tikTokEnable )
            {
                return true;
            }
            return false;
        }
    }
}
