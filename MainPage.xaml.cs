using Streamer_Universal_Chat_Application.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Contacts;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace Streamer_Universal_Chat_Application
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Models.ChatRow> ChatRows { get; }
        = new ObservableCollection<Models.ChatRow>();

        public MainPage()
        {
            this.InitializeComponent();
            Twitch twitch = new Twitch();
            twitch.MessageReceived += newChat;
            twitch.Connected += onLogon;
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
                this.ChatRows.Add(new Models.ChatRow("Assets/twitchicon.png", e.ChatMessage.DisplayName, e.ChatMessage.Message, e.ChatMessage.TmiSentTs, "Assets/tiktokicon.png", "Assets/tiktokicon.png", "Assets/tiktokicon.png"));
            });
        }


        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(Settings));
        }

        private void ItemListView_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void OnElementClicked(object sender, RoutedEventArgs e)
        {
            var selectedFlyoutItem = sender as AppBarButton;

        }
    }
}
