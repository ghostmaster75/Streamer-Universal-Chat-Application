using Streamer_Universal_Chat_Application.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI;
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
        private ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private Twitch()
        {

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

        public async void ConnectToStreamAsync(String twitchUser, String twitchToken, String twitchChannel, Action<Exception> onConnectException = null)
        {
            await ConnectToStream(twitchUser, twitchToken, twitchChannel, onConnectException);
        }

        public async Task ConnectToStream(String twitchUser, String twitchToken, String twitchChannel, Action<Exception> onConnectException = null)
        {
            try
            {
                if (client != null)
                {
                    Debug.WriteLine("Disconnecting Existing Client");
                    await DisconnectFromLivestream();
                }

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
                onConnectException?.Invoke(e);
            }
        }

        public async void DisconnectFromLivestreamAsync() => await DisconnectFromLivestream();

        public async Task DisconnectFromLivestream()
        {
            if (client != null)
            {
                Debug.WriteLine("Disconnecting Client");
                client.Disconnect();
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
            if (e.ChatMessage.Color != null)
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
            String message = e.ChatMessage.Message;

            e.ChatMessage.EmoteSet.Emotes.ForEach(emote =>
            {
                message = message.Replace(emote.Name, emote.ImageUrl);
            });

            if (e.ChatMessage.EmoteReplacedMessage != null)
            {
                message = e.ChatMessage.EmoteReplacedMessage;
            }



            ChatRow chatRow = new ChatRow(Common.Sources.Twitch, Common.Costant.TwitchLogo, e.ChatMessage.DisplayName, e.ChatMessage.Badges, message, e.ChatMessage.TmiSentTs, color);

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

        private void SetupEvents(TwitchClient client)
        {
            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;
        }
        private void TearDownEvents(TwitchClient client)
        {
            client.OnLog -= Client_OnLog;
            client.OnJoinedChannel -= Client_OnJoinedChannel;
            client.OnMessageReceived -= Client_OnMessageReceived;
            client.OnWhisperReceived -= Client_OnWhisperReceived;
            client.OnNewSubscriber -= Client_OnNewSubscriber;
            client.OnConnected -= Client_OnConnected;
        }

    }

}
