﻿using Streamer_Universal_Chat_Application.Common;
using Streamer_Universal_Chat_Application.Models;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static Streamer_Universal_Chat_Application.Common.Events;
using Color = Windows.UI.Color;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace Streamer_Universal_Chat_Application
{
    public sealed partial class MainPage : Page
    {
        private AppSettings appSettings = new AppSettings();
        private ListView chatListView;
        private BuildRow BuildRow = new BuildRow();
        private Twitch twitch;

        public MainPage()
        {
            this.InitializeComponent();
            chatListView = (ListView)FindName("ChatListView");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if ((appSettings.LoadSetting("TwitchUsername") != null && appSettings.LoadSetting("TwitchToken") != null && appSettings.LoadSetting("TwitchChannel") != null) && appSettings.LoadSetting("TwitchEnable") == "True")
            {
                twitch = new Twitch(appSettings.LoadSetting("TwitchUsername"), appSettings.LoadSetting("TwitchToken"), appSettings.LoadSetting("TwitchChannel"));
                twitch.MessageReceived += NewChat;
                twitch.Connected += OnLogon;
                twitch.StatusMessageReceived += OnStatus;
            }

            if (appSettings.LoadSetting("TikTokUserName") != null && appSettings.LoadSetting("TiktokEnable") == "True")
            {
                TikTok tikTok = new TikTok(appSettings.LoadSetting("TikTokUserName"));
                tikTok.Connected += OnLogon;
                tikTok.MessageReceived += NewChat;
                tikTok.StatusMessageReceived += OnStatus;
            }
            this.AppChat("Welcome");

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
            Grid grid = BuildRow.Make(appRow);
            chatListView.Items.Add(grid);
        }


    }
}
