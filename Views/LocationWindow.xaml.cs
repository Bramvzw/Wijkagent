using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using wijkagent.Views;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace wijkagent
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationWindow : Page
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static double[] Coördinates = new double[2];


        public LocationWindow()
        {
            this.InitializeComponent();
        }

        private void Annuleer(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void GetLocation(object sender, RoutedEventArgs e)
        {
            showDialog();
        }

        private void MainMap_MapTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            PositionTextBlockX.Text = MainMap.Center.Position.Latitude.ToString();
            PositionTextBlockY.Text = MainMap.Center.Position.Longitude.ToString();
        }
        // wanneer op selecteer is gedrukt
        private async void showDialog()
        {
            ContentDialog CD = new ContentDialog();
            CD.Title = "Coördinaten gebruiken?";
            CD.Content = $"X: {MainMap.Center.Position.Latitude.ToString()}\nY:{ MainMap.Center.Position.Longitude.ToString()}\n";
            CD.PrimaryButtonText = "Ja";
            CD.SecondaryButtonText = "Nee";

            ContentDialogResult result = await CD.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Coördinates[0] = MainMap.Center.Position.Latitude;
                Coördinates[1] = MainMap.Center.Position.Longitude;
                Frame.GoBack();
            }
            else if (result == ContentDialogResult.Secondary)
            {
                CD.Hide();
            }
        }
    }
}


