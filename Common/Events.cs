using Streamer_Universal_Chat_Application.Models;
using System;

namespace Streamer_Universal_Chat_Application.Common
{
    public class Events
    {
        public class MessageReceivedEventArgs : EventArgs
        {
            public ChatRow ChatRow { get; }

            public MessageReceivedEventArgs(ChatRow chatRow)
            {
                ChatRow = chatRow;
            }
        }

        public class LogEventArgs : EventArgs
        {
            public String Data { get; }

            public LogEventArgs(String data)
            {
                Data = data;
            }
        }

        public class ConnectedEventArgs : EventArgs
        {
            public Boolean IsConnected { get; }

            public ConnectedEventArgs(Boolean isConnected)
            {
                IsConnected = isConnected;
            }
        }


        public class StatusMessageEventArgs : EventArgs
        {
            public String StatusMessage { get; }

            public StatusMessageEventArgs(String statusMessage)
            {
                StatusMessage = statusMessage;
            }
        }

        public class StreamEventArgs : EventArgs
        {
            public String VieverCount { get; }

            public StreamEventArgs(String vieverCount)
            {
                VieverCount = vieverCount;
            }
        }
    }
}
