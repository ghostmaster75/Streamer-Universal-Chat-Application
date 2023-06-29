using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace Streamer_Universal_Chat_Application
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class About : Page
    {
        public string AppTitle { get; set; } = GetAppTitle();
        public string AppVersion { get; set; } = GetAppVersion(); 
        public About()
        {
            this.InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private static string GetAppVersion()
        {
            var package = Windows.ApplicationModel.Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision} - Copyright © 2023 Pierluigi Natale";
        }

        private static string GetAppTitle()
        {
            var package = Windows.ApplicationModel.Package.Current;
            var packageId = package.Id;
            return $"{package.DisplayName}";
        }

    }
}
