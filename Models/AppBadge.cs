using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamer_Universal_Chat_Application.Models
{
    public class AppBadge
    {
        public String backgroundColor { get; set; }
        public String backgroundBorder { get; set; }
        public String imageUrl { get; set; }
        public String text { get; set; }
        public AppBadge(String backgroundColor, String backgroundBorder,  String imageUrl, String text)
        {
            this.backgroundColor = backgroundColor;
            this.backgroundBorder = backgroundBorder;
            this.imageUrl = imageUrl;
            this.text = text;
            this.backgroundBorder = backgroundBorder;
        }
    }
}
