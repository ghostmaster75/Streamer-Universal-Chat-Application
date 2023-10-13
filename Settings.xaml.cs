using Streamer_Universal_Chat_Application.Common;
using System;
using System.Linq;
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
        private String twitchClientId;
        private Boolean tikTokEnable;
        private String tikTokUserName;
        private int maxHistoryChat;
        private AppBarButton saveButton;
        private AppBarButton undoButton;
        private TextBox textBoxMaxHistoryChat;
        private TextBox textBoxTwitchUser;
        private PasswordBox passwordBoxTwitchToken;
        private TextBox textBoxTwitchChannel;
        private TextBox textBoxTwitchClientId;
        private TextBox textBoxTikTokUserName;
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
            textBoxTwitchUser = (TextBox)FindName("TwitchUserName");
            passwordBoxTwitchToken = (PasswordBox)FindName("TwitchAccessToken");
            textBoxTwitchChannel = (TextBox)FindName("TwitchChannel");
            textBoxTwitchClientId = (TextBox)FindName("TwitchClientId");
            toogleSwitchTikTokEnable = (ToggleSwitch)FindName("TiktokEnable");
            textBoxTikTokUserName = (TextBox)FindName("TikTokUserName");
            textBoxMaxHistoryChat = (TextBox)FindName("MaxHistoryLine");

            toogleSwitchTikTokEnable.IsOn = tikTokEnable;
            toogleSwitchTwitchEnable.IsOn = twitchEnable;

            if (maxHistoryChat > 0)
            {
                textBoxMaxHistoryChat.Text = maxHistoryChat.ToString();
            }

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

            if (twitchClientId != null)
            {
                textBoxTwitchClientId.Text = twitchClientId;
            }
            else
            {
                textBoxTwitchClientId.Text = "";
            }

            if (tikTokUserName != null)
            {
                textBoxTikTokUserName.Text = tikTokUserName;
            }
            else
            {
                textBoxTikTokUserName.Text = "";
            }
        }

        private void LoadSettings()
        {
            bool value;
            bool success = bool.TryParse(settings.LoadSetting("TwitchEnable"), out value);
            if (success)
            {
                twitchEnable = value;
            }
            else
            {
                twitchEnable = true; ;
            }

            int number;
            if (int.TryParse(settings.LoadSetting("MaxHistoryChat"), out number))
            {
                maxHistoryChat = number;
            }
            else
            {
                maxHistoryChat = 100;
            }

            twitchUsername = settings.LoadSetting("TwitchUsername");
            twitchToken = settings.LoadSetting("TwitchToken");
            twitchChannel = settings.LoadSetting("TwitchChannel");
            twitchClientId = settings.LoadSetting("TwitchClientId");

            success = bool.TryParse(settings.LoadSetting("TiktokEnable"), out value);
            if (success)
            {
                tikTokEnable = value;
            }
            else
            {
                tikTokEnable = true; ;
            }

            tikTokUserName = settings.LoadSetting("TikTokUserName");

        }

        private void SaveSettings()
        {
            settings.SaveSetting("TwitchEnable", TwitchEnable.IsOn.ToString());
            settings.SaveSetting("TwitchUsername", textBoxTwitchUser.Text);
            settings.SaveSetting("TwitchToken", passwordBoxTwitchToken.Password);
            settings.SaveSetting("TwitchChannel", textBoxTwitchChannel.Text);
            settings.SaveSetting("TwitchClientId", textBoxTwitchClientId.Text);
            settings.SaveSetting("TiktokEnable", TiktokEnable.IsOn.ToString());
            settings.SaveSetting("TikTokUserName", textBoxTikTokUserName.Text.Replace("@", ""));
            settings.SaveSetting("MaxHistoryChat", textBoxMaxHistoryChat.Text);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsSettingsChanged())
            {
                this.SaveSettings();
            }
            Frame.Navigate(typeof(MainPage));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.SaveSettings();
            this.LoadSettings();
            this.DisableButtons();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            toogleSwitchTwitchEnable.IsOn = twitchEnable;

            textBoxTwitchUser.Text = twitchUsername != null ? twitchUsername : "";
            passwordBoxTwitchToken.Password = twitchToken != null ? twitchToken : "" ;
            textBoxTwitchChannel.Text = twitchChannel != null ? twitchChannel : "";
            textBoxTwitchClientId.Text = twitchClientId != null ? twitchClientId : "";
            toogleSwitchTikTokEnable.IsOn = tikTokEnable;
            textBoxTikTokUserName.Text = tikTokUserName != null ? tikTokUserName : "";
            textBoxMaxHistoryChat.Text = maxHistoryChat.ToString();
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
            if (textBoxMaxHistoryChat.Text != maxHistoryChat.ToString()
                || textBoxTwitchUser.Text != twitchUsername 
                || passwordBoxTwitchToken.Password != twitchToken
                || textBoxTwitchChannel.Text != twitchChannel
                || textBoxTwitchClientId.Text != twitchClientId
                || textBoxTikTokUserName.Text != tikTokUserName
                || toogleSwitchTwitchEnable.IsOn != twitchEnable
                || toogleSwitchTikTokEnable.IsOn != tikTokEnable)
            {
                return true;
            }
            return false;
        }

        private void MaxHistoryLine_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;

            // Rimuovi i caratteri non numerici
            string numericText = new string(text.Where(char.IsDigit).ToArray());

            // Aggiorna il testo della TextBox con il risultato numerico
            textBox.Text = numericText;
            textBox.Select(numericText.Length, 0); // Imposta il cursore alla fine del testo

            // Verifica il range numerico
            int minValue = 0; // Valore minimo del range
            int maxValue = 5000; // Valore massimo del range

            if (int.TryParse(numericText, out int numericValue))
            {
                // Verifica se il valore numerico è nel range desiderato
                if (numericValue < minValue || numericValue > maxValue)
                {
                    // Il valore numerico non è nel range, quindi reimposta il testo della TextBox con il valore più vicino nel range
                    textBox.Text = Math.Min(Math.Max(numericValue, minValue), maxValue).ToString();
                }
            }
            this.Settings_Changed(sender, e);
        }
    }
}
