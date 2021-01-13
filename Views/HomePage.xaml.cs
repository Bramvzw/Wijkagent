using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using wijkagent.Core.Services;
using wijkagent.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using wijkagent.Core.Models;
using System.Threading;
using Wijkagent.Ingestor;

namespace wijkagent.Views
{
    public sealed partial class HomePage : Page
    {
        public HomeViewModel ViewModel { get; } = new HomeViewModel();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Categories 
            foreach (var item in SqlDataService.AllCategories())
            {
                Input_Category.Items.Add(item.Name);
            }
            // active and inactive delicts
            ActiveDelicts.Text = $"Actieve delicten: {SqlDataService.ActiveDelicts().ToString()}";
            InactiveDelicts.Text = $"Opgeloste delicten: {SqlDataService.InactiveDelicts().ToString()}";

            // Clock
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += this.Timer_Tick;
            timer.Start();

        }
        // Map pressed
        private void MainMap_MapTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            X.Text = MainMap.Center.Position.Longitude.ToString();
            Y.Text = MainMap.Center.Position.Latitude.ToString();

        }

        // Function for actual time 
        private void Timer_Tick(object sender, object e)
        {
            bRAM.Text = DateTime.Now.ToString();
        }

        // Confirm pressed
        private async void Submit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog CD = new ContentDialog();
            CD.Title = "Delict toevoegen";
            CD.Content = "Weet u zeker dat u dit delict wilt toevoegen?";
            CD.PrimaryButtonText = "Ja";
            CD.SecondaryButtonText = "Nee";

            ContentDialogResult result = await CD.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (double.Parse(Y.Text) <= 90 && double.Parse(Y.Text) >= -90 && double.Parse(X.Text) <= 180 && double.Parse(X.Text) >= -180)
                {
                    double Longtiude = double.Parse(X.Text);
                    double Latitude = double.Parse(Y.Text);
                    List<int> lastActiveDelictIDs = new List<int>();
                    foreach (Delict delict in SqlDataService.AllDelicts())
                    {
                        lastActiveDelictIDs.Add((delict.Id));
                    }

                    if (SqlDataService.SaveDelict(Input_Titel.Text, "none", Input_Category.SelectedIndex, Longtiude, Latitude))
                    {
                        //see which delict(id) is new
                        foreach (Delict delict in SqlDataService.AllDelicts())
                        {
                            if (!lastActiveDelictIDs.Contains(delict.Id)) //if true: it's the new delict
                            {
                                //if getting category and place doesn't succeed it still has a value
                                string categoryName = " ";
                                string placename = " ";
                                //getting category name
                                foreach (Category cat in SqlDataService.AllCategories())
                                {
                                    if (delict.CategoryId == cat.Id)
                                    {
                                        categoryName = cat.Name;
                                    }
                                }
                                //setting a geopostion to get location of delict
                                BasicGeoposition location = new BasicGeoposition();
                                location.Longitude = Longtiude;
                                location.Latitude = Latitude;
                                //geopoint for reverse geocoding
                                Geopoint delictpoint = new Geopoint(location);
                                // Reverse geocode the specified geographic location.
                                MapLocationFinderResult geoResult =
                                    await MapLocationFinder.FindLocationsAtAsync(delictpoint);

                                // If the query returns results, display the name of the town
                                // contained in the address of the first result.
                                if (geoResult.Status == MapLocationFinderStatus.Success)
                                {
                                    if (geoResult.Locations.Count > 0)
                                    {
                                        //placename is the result of the reverse geocoding
                                        placename = geoResult.Locations[0].Address.Town;
                                    }
                                }
                                //giving the keywords string[] relevant information
                                string[] keywords =
                                {
                                    delict.Name
                                };
                                Thread thread = new Thread(() => Ingestor.GetAndStoreTweets(keywords, delict.Id, 10));
                                thread.Start();
                                thread.Join();
                            }
                        }
                        Input_Titel.Text = String.Empty;
                        Input_Category.SelectedIndex = -1;
                        X.Text = String.Empty;
                        Y.Text = String.Empty;
                        LocationWindow.Coördinates[0] = 0;
                        LocationWindow.Coördinates[1] = 0;
                        Frame.Navigate(this.GetType());
                    }
                    else
                    {
                        // empty fields are present
                        ContentDialog Empty = new ContentDialog();
                        Empty.Title = "veld(en) leeggelaten!";
                        Empty.Content = "één of meerdere velden zijn leeggelaten\nVul deze velden in om het toevoegen van het delict te voltooien";
                        Empty.CloseButtonText = "Ok";

                        await Empty.ShowAsync();
                    }
                }
                else
                {
                    // non existing coordinates
                    ContentDialog BadCoords = new ContentDialog();
                    BadCoords.Title = "Verkeerde Coördinaten ingevoerd";
                    BadCoords.Content = "één of beide coördinaten zijn verkeerd ingevoerd!";
                    BadCoords.CloseButtonText = "Ok";

                    await BadCoords.ShowAsync();
                }
            }
            else
            {
                Frame.GoBack();
            }
        }

        // empty fields
        private void Reset_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input_Titel.Text = String.Empty;
            Input_Category.SelectedIndex = -1;
            X.Text = String.Empty;
            Y.Text = String.Empty;
        }

        // open map
        public void Map_PopUp(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LocationWindow));
            X.Text = LocationWindow.Coördinates[0].ToString();
            Y.Text = LocationWindow.Coördinates[1].ToString();
        }
    }
}
