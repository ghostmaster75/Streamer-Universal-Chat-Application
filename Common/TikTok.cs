using Streamer_Universal_Chat_Application.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TikTokLiveSharp.Client;
using TikTokLiveSharp.Events.MessageData.Messages;
using Windows.UI;
using static Streamer_Universal_Chat_Application.Common.Events;

namespace Streamer_Universal_Chat_Application.Common
{
    public class TikTok
    {
        public event EventHandler<StatusMessageEventArgs> StatusMessageReceived;
        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        private static TikTok _instance;
        private readonly String _TikTokUser;
        protected TikTokLiveClient client;
        private ClientSettings settings;
        protected CancellationTokenSource tokenSource = new CancellationTokenSource();

        private TikTok()
        {

        }

        public static TikTok Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TikTok();
                }
                return _instance;
            }
        }

        public async void ConnectToStreamAsync(string userID, Action<Exception> onConnectException = null)
        {
            await ConnectToStream(userID, onConnectException);
        }
        public async Task ConnectToStream(string userID, Action<Exception> onConnectException = null)
        {
            try
            {
                if (client != null)
                {
                    Debug.WriteLine("Disconnecting Existing Client");
                    await DisconnectFromLivestream();
                }

                Debug.WriteLine("Creating new TikTokLiveClient");
                client = new TikTokLiveClient(userID, settings, null);
                Debug.WriteLine($"Created new Client with HostName {userID}");
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
                await client.Start(tokenSource.Token, Client_Error, settings.RetryOnConnectionFailure);
                Debug.WriteLine("Connected");
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
                await client.Stop();
                Debug.WriteLine("Removing EventListeners from Client");
                TearDownEvents(client);
                client = null;
            }
            await Task.Delay(200);
            Debug.WriteLine("Stopping Threads");
            tokenSource?.Cancel();
            tokenSource = new CancellationTokenSource();
        }

        private void Client_Error(object sender, Exception e)
        {
            this.StatusMessage(e.Message);
        }

        private void Client_Error(Exception e)
        {
            this.StatusMessage(e.Message);
        }

        private void Client_OnConnected(TikTokLiveClient sender, bool e)
        {
            OnConnected(new ConnectedEventArgs(e));
            this.StatusMessage("TikTok is connected to room");
            Debug.WriteLine($"Connected to Room! [Connected:{e}]");
        }


        protected virtual void OnConnected(ConnectedEventArgs e)
        {
            Connected?.Invoke(this, e);
        }

        private void Client_OnDisconnected(TikTokLiveClient sender, bool e)
        {
            OnConnected(new ConnectedEventArgs(e));
            this.StatusMessage("TikTok is disconnected");
            Debug.WriteLine($"Disconnected from Room! [Connected:{e}]");
        }

        private static void Client_OnViewerData(TikTokLiveClient sender, RoomViewerData e)
        {
            Debug.WriteLine($"Viewer count is: {e.ViewerCount}");
        }

        private void Client_OnLiveEnded(TikTokLiveClient sender, EventArgs e)
        {
            this.StatusMessage("TikTok host ended stream");
            Debug.WriteLine("Host ended Stream!");
            client.Stop();  
        }

        private static void Client_OnJoin(TikTokLiveClient sender, Join e)
        {
            Debug.WriteLine($"{e.User.UniqueId} joined!");
        }

        private void Client_OnComment(TikTokLiveClient sender, Comment e)
        {
            Debug.WriteLine($"{e.User.UniqueId}: {e.Text}");

            Color color = new Color();            
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            DateTime pointOfReference = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long ticks = (long)(e.TimeStamp / 100);
            var times = pointOfReference.AddTicks(ticks);
            //System.Collections.Generic.List<TikTokLiveSharp.Events.MessageData.Objects.Badge> badges = e.User.Badges;

            List<KeyValuePair<string, string>> badges = new List<KeyValuePair<string, string>>();

            ChatRow chatRow = new ChatRow(Sources.Tiktok, Costant.TikTokLogo, e.User.NickName,badges, e.Text, times.ToString("dd-MM-yyy HH:mm:ss"), color);

            OnMessageReceived(new MessageReceivedEventArgs(chatRow));
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        private static void Client_OnFollow(object sender, Follow e)
        {
            Debug.WriteLine($"{e.NewFollower?.UniqueId} followed!");
        }

        private static void Client_OnShare(TikTokLiveClient sender, Share e)
        {
            Debug.WriteLine($"{e.User?.UniqueId} shared!");
        }

        private static void Client_OnSubscribe(TikTokLiveClient sender, Subscribe e)
        {
            Debug.WriteLine($"{e.NewSubscriber.UniqueId} subscribed!");
        }

        private static void Client_OnLike(TikTokLiveClient sender, Like e)
        {
            Debug.WriteLine($"{e.Sender.UniqueId} liked!");
        }

        private static void Client_OnGiftMessage(TikTokLiveClient sender, GiftMessage e)
        {
            Debug.WriteLine($"{e.Sender.UniqueId} sent {e.Amount}x {e.Gift.Name}!");
        }

        private static void Client_OnEmote(TikTokLiveClient sender, Emote e)
        {
            Debug.WriteLine($"{e.User.UniqueId} sent {e.EmoteId}!");
        }
        private void StatusMessage(String message)
        {
            OnStatusMessage(new StatusMessageEventArgs(message));
        }

        protected virtual void OnStatusMessage(StatusMessageEventArgs e)
        {
            StatusMessageReceived?.Invoke(this, e);
        }

        private static DateTime GetDTCTime(ulong nanoseconds, ulong ticksPerNanosecond)
        {
            DateTime pointOfReference = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long ticks = (long)(nanoseconds / ticksPerNanosecond);
            return pointOfReference.AddTicks(ticks);
        }

        private void SetupEvents(TikTokLiveClient client)
        {
            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;
            client.OnViewerData += Client_OnViewerData;
            client.OnLiveEnded += Client_OnLiveEnded;
            client.OnJoin += Client_OnJoin;
            client.OnComment += Client_OnComment;
            client.OnFollow += Client_OnFollow;
            client.OnShare += Client_OnShare;
            client.OnSubscribe += Client_OnSubscribe;
            client.OnLike += Client_OnLike;
            client.OnGiftMessage += Client_OnGiftMessage;
            client.OnEmote += Client_OnEmote;
        }
        private void TearDownEvents(TikTokLiveClient client)
        {
            client.OnConnected -= Client_OnConnected;
            client.OnDisconnected -= Client_OnDisconnected;
            client.OnViewerData -= Client_OnViewerData;
            client.OnLiveEnded -= Client_OnLiveEnded;
            client.OnJoin -= Client_OnJoin;
            client.OnComment -= Client_OnComment;
            client.OnFollow -= Client_OnFollow;
            client.OnShare -= Client_OnShare;
            client.OnSubscribe -= Client_OnSubscribe;
            client.OnLike -= Client_OnLike;
            client.OnGiftMessage -= Client_OnGiftMessage;
            client.OnEmote -= Client_OnEmote;
        }

    }

}
