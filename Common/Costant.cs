using System;
using System.Collections.Generic;

namespace Streamer_Universal_Chat_Application.Common
{
    static class Costant
    {
        public const String RoundedLogo = "Assets/LogoRounded.png";
        public const String TwitchLogo = "Assets/twitchicon.png";
        public const String TikTokLogo = "Assets/tiktokicon.png";



        public static IReadOnlyDictionary<string, string> TwitchEmojiMappings { get; } = new Dictionary<string, string>()
    {
        { "R)", "https://static-cdn.jtvnw.net/emoticons/v2/14/default/light/1.0" },
        { ";p", "https://static-cdn.jtvnw.net/emoticons/v2/13/default/light/1.0" },
        { ":p", "https://static-cdn.jtvnw.net/emoticons/v2/12/default/light/1.0" },
        { ";)", "https://static-cdn.jtvnw.net/emoticons/v2/11/default/light/1.0" },
        { ":\\", "https://static-cdn.jtvnw.net/emoticons/v2/10/default/light/1.0" },
        { "<3", "https://static-cdn.jtvnw.net/emoticons/v2/9/default/light/1.0" },
        { ":o", "https://static-cdn.jtvnw.net/emoticons/v2/8/default/light/1.0" },
        { "B)", "https://static-cdn.jtvnw.net/emoticons/v2/7/default/light/1.0" },
        { "O_o", "https://static-cdn.jtvnw.net/emoticons/v2/6/default/light/1.0" },
        { ":|", "https://static-cdn.jtvnw.net/emoticons/v2/5/default/light/1.0" },
        { ">(", "https://static-cdn.jtvnw.net/emoticons/v2/4/default/light/1.0" },
        { ":D", "https://static-cdn.jtvnw.net/emoticons/v2/3/default/light/1.0" },
        { ":(", "https://static-cdn.jtvnw.net/emoticons/v2/2/default/light/1.0" },
        { ":)", "https://static-cdn.jtvnw.net/emoticons/v2/1/default/light/1.0" }
    };

    }

    public enum Sources
    {
        Application, Twitch, Tiktok, Instagra, Discord
    }
}
