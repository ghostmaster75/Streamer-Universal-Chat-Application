using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Streamer_Universal_Chat_Application.Models
{
    public class ChatRow
    {
        public String ImageSource { get; private set; }
        public String NickName { get; private set; }
        public String ChatText { get; private set; }
        public String MsgDateTime { get; private set; }
        public String Badge1 { get; private set; }
        public String Badge2 { get; private set; }
        public String Badge3 { get; private set; }

        public ChatRow(String imageSource, String nickNAme, String chatText, String dateTime, String badge1, String badge2, String badge3) {
            ImageSource = imageSource;
            NickName = nickNAme;
            ChatText = chatText;
            MsgDateTime = dateTime;
            Badge1 = badge1;    
            Badge2 = badge2;    
            Badge3 = badge3;    
        }
    }
}
