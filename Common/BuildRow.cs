using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Streamer_Universal_Chat_Application.Models;
using System.Diagnostics;

namespace Streamer_Universal_Chat_Application.Common
{
    internal class BuildRow
    {
        private AppSettings appSettings = new AppSettings();
        public BuildRow() { }

        private void ProcessEmoji(Sources source, Boolean isNickColor, Color color, string text, RichTextBlock richTextBlock)
        {
            IReadOnlyDictionary<string, string> emoMapping = new Dictionary<string, string>();
            
            if (source == Sources.Twitch) {
                emoMapping = Costant.TwitchEmojiMappings;
            }

            string[] words = text.Split(' ');

            Paragraph paragraph = new Paragraph();

            if (isNickColor)
            {
                paragraph.FontWeight = FontWeights.Bold;
            }

            foreach (string word in words)
            {
                if (emoMapping.ContainsKey(word))
                {
                    // Emoji found, add image to paragraph
                    Image emojiImage = new Image();
                    emojiImage.Source = new BitmapImage(new Uri(emoMapping[word]));
                    emojiImage.Width = 20;
                    emojiImage.Height = 20;

                    InlineUIContainer container = new InlineUIContainer();
                    container.Child = emojiImage;

                    paragraph.Inlines.Add(container);
                    paragraph.Inlines.Add(new Run() { Text = " " }); // Add space to separate the emoji from the next word
                }
                else if (word.Contains("emoticon") || word.Contains("badges"))
                {
                    // URL containing "emoticon" found, add image to paragraph
                    if (!string.IsNullOrEmpty(word))
                    {
                        Image emoticonImage = new Image();
                        emoticonImage.Source = new BitmapImage(new Uri(word));
                        emoticonImage.Width = 20;
                        emoticonImage.Height = 20;

                        InlineUIContainer container = new InlineUIContainer();
                        container.Child = emoticonImage;

                        paragraph.Inlines.Add(container);
                        paragraph.Inlines.Add(new Run() { Text = " " }); // Add space to separate the emoticon from the next word
                    }
                }
                else
                {
                    // Regular word, add to paragraph
                    paragraph.Inlines.Add(new Run() { Text = word + " ", Foreground = new SolidColorBrush(color) });
                }
            }

            richTextBlock.Blocks.Add(paragraph);
        }

        public Grid Make(ChatRow chatRow)
        {
            Grid grid = new Grid();
            grid.Padding = new Thickness(6);
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(1.0, GridUnitType.Star);
            grid.RowDefinitions.Add(rowDefinition);

            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1.0, GridUnitType.Auto);
            grid.ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1.0, GridUnitType.Star);
            grid.ColumnDefinitions.Add(columnDefinition);

            Ellipse ellipse = new Ellipse();
            ellipse.Width = 32;
            ellipse.Height = 32;
            ellipse.Margin = new Thickness(6);
            ImageBrush sourceIcon = new ImageBrush();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri("ms-appx:" + chatRow.ImageSource);
            sourceIcon.ImageSource = bitmapImage;
            ellipse.Fill = sourceIcon;
            Grid.SetColumn(ellipse, 0);
            grid.Children.Add(ellipse);

            RichTextBlock chatText = new RichTextBlock();
            chatText.FontSize = Double.Parse(appSettings.LoadSetting("fontsize"));
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            Color color;
            if (chatRow.UserColor != null)
            {
                color = chatRow.UserColor;
            }
            else
            {
                color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            }

            this.ProcessEmoji(chatRow.Source, true, color, chatRow.NickName, chatText);

            color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Foreground);
            this.ProcessEmoji(chatRow.Source, false, color, chatRow.ChatText, chatText);
            Grid.SetColumn(chatText, 1);
            grid.Children.Add(chatText);

            return grid;
        }
    }
}
