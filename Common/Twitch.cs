using Streamer_Universal_Chat_Application.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Chat.Badges;
using TwitchLib.Api.Helix.Models.Chat.Badges.GetGlobalChatBadges;
using TwitchLib.Api.Helix.Models.Streams.GetFollowedStreams;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using static Streamer_Universal_Chat_Application.Common.Events;

namespace Streamer_Universal_Chat_Application.Common
{
    public class Twitch
    {
        private TwitchClient client;
        private static Twitch _instance;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<LogEventArgs> Log;
        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<StatusMessageEventArgs> StatusMessageReceived;
        public event EventHandler<StreamEventArgs> StreamEvent;
        private ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private static TwitchAPI api;
        //private GetFollowedStreamsResponse channelFollowers;
        private GetStreamsResponse streams;
        private String _twitchToken;
        private String _twitchUser;
        private DispatcherTimer timer;
        private GetGlobalChatBadgesResponse globalBadges;

        private Twitch()
        {
            api = new TwitchAPI();
        }

        public static Twitch Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Twitch();
                }
                return _instance;
            }
        }

        public async void ConnectToStreamAsync(String twitchUser, String twitchToken, String twitchChannel, String twitchClientId, Action<Exception> onConnectException = null)
        {
            try
            {
                await ConnectToStream(twitchUser, twitchToken, twitchChannel, twitchClientId, onConnectException);
                await GetBadges();
            }
            catch (Exception e)
            {
                this.StatusMessage($"Twitch error: {e.Message}");
                onConnectException?.Invoke(e);
            }

        }

        public async Task ConnectToStream(String twitchUser, String twitchToken, String twitchChannel, String twitchClientId, Action<Exception> onConnectException = null)
        {
            try
            {
                if (client != null)
                {
                    Debug.WriteLine("Disconnecting Existing Client");
                    await DisconnectFromLivestream();
                }

                _twitchToken = twitchToken;
                _twitchUser = twitchUser;

                api.Settings.ClientId = twitchClientId;
                api.Settings.AccessToken = twitchToken;

                Debug.WriteLine("Creating new Twitch Cllient");
                ConnectionCredentials credentials = new ConnectionCredentials(twitchUser, twitchToken);
                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                };
                WebSocketClient customClient = new WebSocketClient(clientOptions);
                client = new TwitchClient(customClient);
                client.WillReplaceEmotes = true;
                client.Initialize(credentials, twitchChannel);
                Debug.WriteLine($"Created new Client with HostName {twitchChannel}");
                Debug.WriteLine("Connecting Events to Client");

                SetupEvents(client);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                this.StatusMessage($"Twitch error: {e.Message}");
                onConnectException?.Invoke(e);
                return;
            }
            try
            {
                client.Connect();
                Debug.WriteLine("Twitch Connected");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                this.StatusMessage($"Twitch error: {e.Message}");
                onConnectException?.Invoke(e);
            }
        }

        public async void DisconnectFromLivestreamAsync() => await DisconnectFromLivestream();

        public async Task DisconnectFromLivestream()
        {
            if (client != null)
            {
                Debug.WriteLine("Disconnecting Client");
                //client.Disconnect();
                Debug.WriteLine("Removing EventListeners from Client");
                TearDownEvents(client);
                client = null;
            }
            await Task.Delay(200);
            Debug.WriteLine("Stopping Threads");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            OnLog(new LogEventArgs(e.Data));
            Debug.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        protected virtual void OnLog(LogEventArgs e)
        {
            Log?.Invoke(this, e);
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            OnConnected(new ConnectedEventArgs(true));
            this.StatusMessage(_resourceLoader.GetString("TwitchConnected"));
            Debug.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        protected virtual void OnConnected(ConnectedEventArgs e)
        {
            Connected?.Invoke(this, e);
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            this.StatusMessage(_resourceLoader.GetString("TwitchJoined") + " " + e.Channel);
            //client.SendMessage(e.Channel, "Hey guys! I am a bot connected via Streamer Universal Chat Application");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Color color = new Color();
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            if (e.ChatMessage.Color != null && e.ChatMessage.Color.Name != "0")
            {
                // questo è un bug della libreria di twich che mette il valore di A alla fine della stringa invece che all'inizio
                byte R = Convert.ToByte(e.ChatMessage.Color.Name.Substring(0, 2), 16);
                byte G = Convert.ToByte(e.ChatMessage.Color.Name.Substring(2, 2), 16);
                byte B = Convert.ToByte(e.ChatMessage.Color.Name.Substring(4, 2), 16);
                color = Color.FromArgb(255, R, G, B);
            }
            else
            {
                color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            }

            var nickName = "";
            e.ChatMessage.Badges.ForEach(badge =>
            {
                BadgeEmoteSet[] emoteSet = globalBadges.EmoteSet;
                BadgeEmoteSet matchingSet = emoteSet.FirstOrDefault(badgeImage => badgeImage.SetId == badge.Key);

                if (matchingSet != null)
                {
                    nickName += matchingSet.Versions[0].ImageUrl1x + " ";
                }
            });
            nickName += e.ChatMessage.DisplayName;

            String message = e.ChatMessage.Message;

            e.ChatMessage.EmoteSet.Emotes.ForEach(emote =>
            {
                message = message.Replace(emote.Name, emote.ImageUrl);
            });

            if (e.ChatMessage.EmoteReplacedMessage != null)
            {
                message = e.ChatMessage.EmoteReplacedMessage;
            }



            ChatRow chatRow = new ChatRow(Common.Sources.Twitch, Common.Costant.TwitchLogo, nickName, null, message, e.ChatMessage.TmiSentTs, color);

            OnMessageReceived(new MessageReceivedEventArgs(chatRow));

            //if (e.ChatMessage.Message.Contains("badword"))
            //    client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            // Verifica se ci sono handler registrati per l'evento
            MessageReceived?.Invoke(this, e);
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            //if (e.WhisperMessage.Username == "my_friend")
            //    client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {

            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
            {
                this.StatusMessage($"🌟 {e.Subscriber.DisplayName} {_resourceLoader.GetString("SubscriberPrime")}");
                //client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            }
            else
            {
                this.StatusMessage($"🌟{e.Subscriber.DisplayName} {_resourceLoader.GetString("Subscriber")}");
                //client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
            }
        }

        private void StatusMessage(String message)
        {
            OnStatusMessage(new StatusMessageEventArgs(message));
        }

        protected virtual void OnStatusMessage(StatusMessageEventArgs e)
        {
            // Verifica se ci sono handler registrati per l'evento
            StatusMessageReceived?.Invoke(this, e);
        }

        public async Task GetLiveInfo()
        {
            List<string> userLists = new List<string> { _twitchUser };
            try
            {
                streams = await api.Helix.Streams.GetStreamsAsync(userLogins: userLists, accessToken: _twitchToken);

                if (streams != null && streams.Streams.Length > 0)
                {
                    TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream stream = streams.Streams[0];
                    Debug.WriteLine($"Viewer : {stream.ViewerCount}");
                    OnLiveInfo(new StreamEventArgs(stream.ViewerCount.ToString()));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            //            else {
            //                this.StatusMessage($"{ _resourceLoader.GetString("TwitchNoLive")}");
            //            }
        }


        private async Task GetBadges()
        {
            try
            {
                globalBadges = await api.Helix.Chat.GetGlobalChatBadgesAsync(accessToken: _twitchToken);
            } catch (Exception ex)
            {
                Debug.WriteLine($"Twitch error {ex.Message}");
            }

            Debug.WriteLine(" ");
        }

        private async void GetLiveInfoAsync()
        {
            await GetLiveInfo();
        }

        protected virtual void OnLiveInfo(StreamEventArgs e)
        {
            StreamEvent?.Invoke(this, e);
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            this.GetLiveInfoAsync();
        }

        private void Client_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            this.GetLiveInfoAsync();
        }



        private void SetupEvents(TwitchClient client)
        {
            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            //client.OnGiftedSubscription += Client_GiftedSubscription;
            //client.OnRaidNotification += Client_OnRaid;
            // TODO: IMPLEMENTARE RESUSCRIBE - RAID - FOLLOW
            client.OnConnected += Client_OnConnected;
            client.OnUserJoined += Client_OnUserJoined;
            client.OnUserLeft += Client_OnUserLeft;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            Debug.WriteLine("Tik");
            this.GetLiveInfoAsync();
        }

        private void TearDownEvents(TwitchClient client)
        {
            client.OnLog -= Client_OnLog;
            client.OnJoinedChannel -= Client_OnJoinedChannel;
            client.OnMessageReceived -= Client_OnMessageReceived;
            client.OnWhisperReceived -= Client_OnWhisperReceived;
            client.OnNewSubscriber -= Client_OnNewSubscriber;
            client.OnConnected -= Client_OnConnected;
            client.OnUserJoined -= Client_OnUserJoined;
            client.OnUserLeft -= Client_OnUserLeft;
            timer.Tick -= Timer_Tick;
            timer.Stop();
        }

    }

}
