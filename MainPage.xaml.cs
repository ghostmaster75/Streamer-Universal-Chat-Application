using Streamer_Universal_Chat_Application.Common;
using Streamer_Universal_Chat_Application.Models;
using System;
using System.Collections.Generic;
using System.Resources;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static Streamer_Universal_Chat_Application.Common.Events;
using Color = Windows.UI.Color;

namespace Streamer_Universal_Chat_Application
{
    public sealed partial class MainPage : Page
    {
        private AppSettings appSettings = new AppSettings();
        private ListView chatListView;
        private BuildRow BuildRow = new BuildRow();
        private Twitch _twitch = Twitch.Instance;
        private TikTok _tiktok = TikTok.Instance;
        private ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private double _fontsize;

        public MainPage()
        {
            this.InitializeComponent();
            chatListView = (ListView)FindName("ChatListView");
            if (appSettings.LoadSetting("fontsize") != null)
            {
                _fontsize = Double.Parse(appSettings.LoadSetting("fontsize"));
            }
            else {
                _fontsize = 18;
                appSettings.SaveSetting("fontsize",_fontsize.ToString());
            }

            if ((appSettings.LoadSetting("TwitchUsername") != null && appSettings.LoadSetting("TwitchToken") != null && appSettings.LoadSetting("TwitchChannel") != null) && appSettings.LoadSetting("TwitchEnable") == "True")
            {
                _twitch.ConnectToStreamAsync(appSettings.LoadSetting("TwitchUsername"), appSettings.LoadSetting("TwitchToken"), appSettings.LoadSetting("TwitchChannel"));
                _twitch.MessageReceived += NewChat;
                _twitch.Connected += OnLogon;
                _twitch.StatusMessageReceived += OnStatus;
            }

            if (appSettings.LoadSetting("TikTokUserName") != null && appSettings.LoadSetting("TiktokEnable") == "True")
            {
                _tiktok.ConnectToStreamAsync(appSettings.LoadSetting("TikTokUserName"));
                _tiktok.Connected += OnLogon;
                _tiktok.MessageReceived += NewChat;
                _tiktok.StatusMessageReceived += OnStatus;
            }
            this.AppChat(_resourceLoader.GetString("WelcomeMessage"));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private async void OnStatus(object sender, StatusMessageEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.StatusMessage != null)
                {
                    this.AppChat(e.StatusMessage);
                }
            });
        }



        private async void OnLogon(object sender, ConnectedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (sender is Twitch)
                {
                    if (e.IsConnected)
                    {
                        TwitchLed.Fill = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        TwitchLed.Fill = new SolidColorBrush(Colors.Red);
                    }
                }
                if (sender is TikTok)
                {
                    if (e.IsConnected)
                    {
                        TikTokLed.Fill = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        TikTokLed.Fill = new SolidColorBrush(Colors.Red);
                    }
                }

            });
        }

        private async void NewChat(object sender, MessageReceivedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (e.ChatRow.ChatText != null)
                {
                    Grid grid = BuildRow.Make(e.ChatRow);
                    chatListView.Items.Add(grid);
                }
            });
        }

        private void OnElementClicked(object sender, RoutedEventArgs e)
        {
            var selectedFlyoutItem = sender as AppBarButton;
            IEnumerable<RichTextBlock> richTextBlocks = FindChildren<RichTextBlock>(this);

            switch (selectedFlyoutItem.Name)
            {
                case "settings":
                    Frame.Navigate(typeof(Settings));
                    break;
                case "about":
                    Frame.Navigate(typeof(About));
                    break;
                case "fontplus":
                    _fontsize++;
                    foreach(RichTextBlock richTextBlock in richTextBlocks)
{
                        richTextBlock.FontSize = _fontsize; // Imposta la nuova FontSize desiderata
                    }
                    appSettings.SaveSetting("fontsize", _fontsize.ToString());
                    break;
                case "fontminus":
                    _fontsize--;
                    if (_fontsize < 1.0) { 
                        _fontsize = 1;
                    }
                    foreach (RichTextBlock richTextBlock in richTextBlocks)
                    {
                        richTextBlock.FontSize = _fontsize; // Imposta la nuova FontSize desiderata
                    }
                    appSettings.SaveSetting("fontsize", _fontsize.ToString());
                    break;

            }
        }

        private void AppChat(String Message)
        {
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            Color color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            List<KeyValuePair<string, string>> badges = new List<KeyValuePair<string, string>>();
            ChatRow appRow = new ChatRow(Common.Sources.Application, Common.Costant.RoundedLogo, "Streamer Universal Chat Application", badges, Message, "0", color);
            Grid grid = BuildRow.Make(appRow);
            chatListView.Items.Add(grid);
        }

        public static IEnumerable<T> FindChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T descendant in FindChildren<T>(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

    }
}
