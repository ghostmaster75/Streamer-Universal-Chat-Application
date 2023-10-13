using Streamer_Universal_Chat_Application.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Streamer_Universal_Chat_Application.Common
{
    internal class BuildRow
    {
        private AppSettings appSettings = new AppSettings();
        public BuildRow() { }

        private void ProcessEmojiAndBadges(Sources source, Boolean isNickColor, Color color, string text, List<AppBadge> appBadges, RichTextBlock richTextBlock)
        {
            IReadOnlyDictionary<string, string> emoMapping = new Dictionary<string, string>();

            if (source == Sources.Twitch)
            {
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
                else if (word.Contains("emoticon") || word.Contains("badges") || word.Contains("{badgeimage:"))
                {
                    // URL containing "emoticon" found, add image to paragraph
                    if (!string.IsNullOrEmpty(word))
                    {
                        Image emoticonImage = new Image();
                        emoticonImage.Source = new BitmapImage(new Uri(word));
                        emoticonImage.Width = 12;
                        emoticonImage.Height = 12;

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
            if (appBadges != null && appBadges.Count > 0)
            {
                InlineUIContainer badgeContainer = new InlineUIContainer();
                foreach (AppBadge appBadge in appBadges)
                {
                    badgeContainer.FontSize = 12;

                    if (appBadge.backgroundColor != null && appBadge.backgroundColor.Length > 0)
                    {
                        if (appBadge.backgroundColor.StartsWith("#"))
                        {
                            appBadge.backgroundColor = appBadge.backgroundColor.Substring(1);
                        }
                    }
                    else
                    {
                        appBadge.backgroundColor = "803F3F3F";
                    }

                    byte a = byte.Parse(appBadge.backgroundColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    byte r = byte.Parse(appBadge.backgroundColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    byte g = byte.Parse(appBadge.backgroundColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    byte b = byte.Parse(appBadge.backgroundColor.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

                    Color bgColor = Color.FromArgb(a, r, g, b);

                    if (appBadge.backgroundBorder != null && appBadge.backgroundBorder.Length > 0)
                    {
                        if (appBadge.backgroundBorder.StartsWith("#"))
                        {
                            appBadge.backgroundBorder = appBadge.backgroundBorder.Substring(1);
                        }
                    }
                    else
                    {
                        appBadge.backgroundBorder = appBadge.backgroundColor;
                    }

                    byte ab = byte.Parse(appBadge.backgroundBorder.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    byte rb = byte.Parse(appBadge.backgroundBorder.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    byte gb = byte.Parse(appBadge.backgroundBorder.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    byte bb = byte.Parse(appBadge.backgroundBorder.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                    Color bColor = Color.FromArgb(ab, rb, gb, bb);

                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Horizontal;

                    Border border = new Border();
                    border.Background = new SolidColorBrush(bgColor);
                    border.BorderBrush = new SolidColorBrush(bColor);
                    border.CornerRadius = new CornerRadius(4);
                    border.BorderThickness = new Thickness(3,0,3,0);


                    if (appBadge.imageUrl != null)
                    {
                        Image badgeImage = new Image();
                        badgeImage.Source = new BitmapImage(new Uri(appBadge.imageUrl));
                        badgeImage.Width = 12;
                        badgeImage.Height = 12;
                        stackPanel.Children.Add(badgeImage);
                    }

         

                    TextBlock textBlock = new TextBlock();
                    if (appBadge.text != null && appBadge.text.Length > 0)
                    {
                        textBlock.Text = appBadge.text;
                    }
                    textBlock.FontSize = 12;
                    textBlock.Padding = new Thickness(0);
                    stackPanel.Children.Add(textBlock);
                    

                    border.Child= stackPanel;
                    InlineUIContainer stackPanelContainer = new InlineUIContainer();
                    stackPanelContainer.Child = border;
                    paragraph.Inlines.Add(stackPanelContainer);
                    paragraph.Inlines.Add(new Run() { Text = " " });
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

            this.ProcessEmojiAndBadges(chatRow.Source, true, color, chatRow.NickName, chatRow.Badges, chatText );

            color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Foreground);
            this.ProcessEmojiAndBadges(chatRow.Source, false, color, chatRow.ChatText, null, chatText);
            Grid.SetColumn(chatText, 1);
            grid.Children.Add(chatText);

            return grid;
        }
    }
}
