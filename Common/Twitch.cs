using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using TwitchLib.Communication.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using System.Collections.ObjectModel;
using TwitchLib.Client.Enums;
using Windows.UI.Xaml.Controls;
using Windows.System;
using System.Runtime.CompilerServices;

namespace Streamer_Universal_Chat_Application.Common
{
    class Twitch
    {
        private TwitchClient client;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<LogEventArgs> Log;
        public event EventHandler<ConnectedEventArgs> Connected;


        public Twitch()
        {
            ConnectionCredentials credentials = new ConnectionCredentials("ghostshadow75", "gpe7kx4lmb34wj97w0xv0kfw22pt7k");
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, "ghostshadow75");

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            OnLog(new LogEventArgs(e.Data));
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        protected virtual void OnLog(LogEventArgs e) {
            Log?.Invoke(this, e);
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            OnConnected(new ConnectedEventArgs(true));
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        protected virtual void OnConnected(ConnectedEventArgs e)
        {
            Connected?.Invoke(this, e);
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            client.SendMessage(e.Channel, "Hey guys! I am a bot connected via Streamer Universal Chat Application");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            OnMessageReceived(new MessageReceivedEventArgs(e.ChatMessage));
            if (e.ChatMessage.Message.Contains("badword"))
                client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            // Verifica se ci sono handler registrati per l'evento
            MessageReceived?.Invoke(this, e);
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }
    }

    class MessageReceivedEventArgs : EventArgs
    {
        public ChatMessage ChatMessage { get; }

        public MessageReceivedEventArgs(ChatMessage chatMessage)
        {
            ChatMessage = chatMessage;
        }
    }

    class LogEventArgs : EventArgs
    {
        public String Data { get; }

        public LogEventArgs(String data)
        {
            Data = data;
        }
    }

    class ConnectedEventArgs : EventArgs
    {
        public Boolean IsConnected { get; }

        public ConnectedEventArgs(Boolean isConnected)
        {
            IsConnected = isConnected;
        }
    }

}
