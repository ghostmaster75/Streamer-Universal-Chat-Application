using Streamer_Universal_Chat_Application.Common;
using Streamer_Universal_Chat_Application.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Helix;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using static Streamer_Universal_Chat_Application.Common.Events;
using Color = Windows.UI.Color;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace Streamer_Universal_Chat_Application
{
    public sealed partial class MainPage : Page
    {
        private AppSettings appSettings = new AppSettings();
        private TextBlock textBlockStatusMessage;
        ListView chatListView;
        private BuildRow buildRow = new BuildRow();


        public MainPage()
        {
            this.InitializeComponent();
            chatListView = (ListView)FindName("ChatListView");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            textBlockStatusMessage = (TextBlock)FindName("TextBlockStatusMessage");
            if ((appSettings.LoadSetting("TwitchUsername") != null && appSettings.LoadSetting("TwitchToken") != null && appSettings.LoadSetting("TwitchChannel") != null) && appSettings.LoadSetting("TwitchEnable") == "True")
            {
                Twitch twitch = new Twitch(appSettings.LoadSetting("TwitchUsername"), appSettings.LoadSetting("TwitchToken"), appSettings.LoadSetting("TwitchChannel"));
                twitch.MessageReceived += newChat;
                twitch.Connected += onLogon;
                twitch.StatusMessageReceived += onStatus;
            }

            if (appSettings.LoadSetting("TikTokUserNane") != null && appSettings.LoadSetting("TiktokEnable") == "True")
            {
                TikTok tikTok = new TikTok(appSettings.LoadSetting("TikTokUser"));
                tikTok.StatusMessageReceived += onStatus;
            }
            this.AppChat("Welcome");

        }

        private async void onStatus(object sender, StatusMessageEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.StatusMessage != null)
                {
                    this.AppChat(e.StatusMessage);
                }
            });
        }



        private async void onLogon(object sender, ConnectedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.IsConnected)
                {
                    TwitchLed.Fill = new SolidColorBrush(Colors.Green);
                }
            });
        }

        private async void newChat(object sender, MessageReceivedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.ChatRow.ChatText != null)
                {
                    Grid grid = buildRow.Make(e.ChatRow);
                    chatListView.Items.Add(grid);
                }
            });
        }

        private void OnElementClicked(object sender, RoutedEventArgs e)
        {
            var selectedFlyoutItem = sender as AppBarButton;
            if (selectedFlyoutItem.Name == "settings")
            {
                Frame.Navigate(typeof(Settings));
            }
            if (selectedFlyoutItem.Name == "about")
            {
                Frame.Navigate(typeof(About));
            }
        }

        private void AppChat(String Message)
        {
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            Color color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            List<KeyValuePair<string, string>> badges = new List<KeyValuePair<string, string>>();
            ChatRow appRow = new ChatRow(Common.Sources.Application, Common.Costant.RoundedLogo, "Streamer Universal Chat Application", badges, Message, "0", color);
            Grid grid = buildRow.Make(appRow);
            chatListView.Items.Add(grid);
        }


    }
}
