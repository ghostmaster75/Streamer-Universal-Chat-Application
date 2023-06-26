using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Streamer_Universal_Chat_Application.Models
{
    public class ChatRow
    {
        public Common.Sources Source { get; private set; }
        public String ImageSource { get; private set; }
        public String NickName { get; private set; }

        public List<KeyValuePair<string, string>> Badges;
        public String ChatText { get; set; }
        public String MsgDateTime { get; private set; }
        public Color UserColor { get; private set; }
        public SolidColorBrush BrushColor { get => new SolidColorBrush(UserColor); }

        public ChatRow(Common.Sources source, String imageSource, String nickNAme, List<KeyValuePair<string, string>> badges, String chatText, String dateTime, Color userColor)
        {
            Source = source;
            ImageSource = imageSource;
            NickName = nickNAme;
            Badges = badges;
            ChatText = chatText;
            MsgDateTime = dateTime;
            UserColor = userColor;
        }
    }
}
