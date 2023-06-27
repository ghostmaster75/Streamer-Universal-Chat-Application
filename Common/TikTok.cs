using Streamer_Universal_Chat_Application.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TikTokLiveSharp.Client;
using TikTokLiveSharp.Events.MessageData.Messages;
using Windows.UI;
using static Streamer_Universal_Chat_Application.Common.Events;

namespace Streamer_Universal_Chat_Application.Common
{
    internal class TikTok
    {
        public event EventHandler<StatusMessageEventArgs> StatusMessageReceived;
        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        private String TikTokUser;
        private readonly TikTokLiveClient client;

        public TikTok(String tikTokUser)
        {
            TikTokUser = tikTokUser;
            client = new TikTokLiveClient(tikTokUser);
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
            try
            {
                client.Start(new System.Threading.CancellationToken());
                //client.Run(new System.Threading.CancellationToken());
            }
            catch (Exception e)
            {
                this.StatusMessage(e.Message);
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }


        private void Client_OnConnected(TikTokLiveClient sender, bool e)
        {
            OnConnected(new ConnectedEventArgs(true));
            this.StatusMessage("TikTok is connected to room");
            Debug.WriteLine($"Connected to Room! [Connected:{e}]");
        }


        protected virtual void OnConnected(ConnectedEventArgs e)
        {
            Connected?.Invoke(this, e);
        }

        private void Client_OnDisconnected(TikTokLiveClient sender, bool e)
        {
            OnConnected(new ConnectedEventArgs(true));
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

            ChatRow chatRow = new ChatRow(Common.Sources.Tiktok, Common.Costant.TikTokLogo, e.User.NickName,badges, e.Text, times.ToString("dd-MM-yyy HH:mm:ss"), color);

            OnMessageReceived(new MessageReceivedEventArgs(chatRow));
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            // Verifica se ci sono handler registrati per l'evento
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
            // Verifica se ci sono handler registrati per l'evento
            StatusMessageReceived?.Invoke(this, e);
        }

        private static DateTime GetDTCTime(ulong nanoseconds, ulong ticksPerNanosecond)
        {
            DateTime pointOfReference = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long ticks = (long)(nanoseconds / ticksPerNanosecond);
            return pointOfReference.AddTicks(ticks);
        }

        private static DateTime GetDTCTime(ulong nanoseconds)
        {
            return GetDTCTime(nanoseconds, 100);
        }

    }


}
