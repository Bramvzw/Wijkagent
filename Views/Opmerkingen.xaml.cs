using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using wijkagent.Core.Services;
using wijkagent.Views;
using System.Diagnostics;




// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace wijkagent.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Opmerkingen : Page
    {
        ObservableCollection<Add_Suspect.Suspect> suspects = Add_Suspect.Suspects;
        ObservableCollection<Add_Time.Time_Loc> data = Add_Time.Data;
        ObservableCollection<Add_Vehicle.Vehicle> vehicles = Add_Vehicle.Vehicles;

        public Opmerkingen()
        {
            this.InitializeComponent();
            ObservableCollection<Add_Time.Time_Loc> data = Add_Time.Data;
            ObservableCollection<Add_Vehicle.Vehicle> vehicles = Add_Vehicle.Vehicles;

            Suspect_Listview.ItemsSource = suspects;
            Time_Listview.ItemsSource = data;
            Vehicle_Listview.ItemsSource = vehicles;
        }
        public async void Add_Supects(object sender, RoutedEventArgs e)
        {
            Add_Suspect add_Suspect = new Add_Suspect() { };
            await add_Suspect.ShowAsync();
            ObservableCollection<Add_Suspect.Suspect> suspects = Add_Suspect.Suspects;
            Suspect_Listview.ItemsSource = suspects;
        }

        private async void Add_Times(object sender, RoutedEventArgs e)
        {
            Add_Time add_Time = new Add_Time() { };
            await add_Time.ShowAsync();
            ObservableCollection<Add_Time.Time_Loc> time = Add_Time.Data;
            Time_Listview.ItemsSource = time;
        }

        private async void Add_Vehicles(object sender, RoutedEventArgs e)
        {
            Add_Vehicle add_Vehicle = new Add_Vehicle();
            await add_Vehicle.ShowAsync();
            ObservableCollection<Add_Vehicle.Vehicle> vehicles = Add_Vehicle.Vehicles;
            Vehicle_Listview.ItemsSource = vehicles;
        }

        //private async void Toevoegen(object sender, RoutedEventArgs e)
        //{
        //    SqlDataService.SaveDescriptionSuspect(suspect.In_Age, suspect.In_Length, suspect.In_SkinColor, suspect.In_Gender);
        //}


        private async void UpdateSuspect(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            int index = Suspect_Listview.Items.IndexOf(item);

            Add_Suspect add_suspect = new Add_Suspect();
            Add_Suspect.Suspect suspect = new Add_Suspect.Suspect();
            suspect.In_Age = suspects[index].In_Age;
            suspect.In_Length = suspects[index].In_Length;
            suspect.In_SkinColor = suspects[index].In_SkinColor;
            suspect.In_Gender = suspects[index].In_Gender;
            add_suspect.Update(add_suspect, suspect);


            await add_suspect.ShowAsync();

            if (add_suspect.result == Add_Suspect.Result.Toevoegen)
            {
                suspects.Remove(suspects[index]);
            }
        }
        private async void UpdateTime(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            int index = Time_Listview.Items.IndexOf(item);

            Add_Time add_Time = new Add_Time();
            Add_Time.Time_Loc time_Loc = new Add_Time.Time_Loc();
            time_Loc.In_Time = data[index].In_Time;
            time_Loc.In_Date = data[index].In_Date;
            time_Loc.In_Location = data[index].In_Location;
            add_Time.Update(add_Time, time_Loc);

            await add_Time.ShowAsync();

            if (add_Time.result == Add_Time.Result.Toevoegen)
            {
                data.Remove(data[index]);
            }
        }
        private async void UpdateVehicle(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            int index = Vehicle_Listview.Items.IndexOf(item);

            Add_Vehicle add_Vehicle = new Add_Vehicle();
            Add_Vehicle.Vehicle vehicle = new Add_Vehicle.Vehicle();
            vehicle.In_Plate = vehicles[index].In_Plate;
            vehicle.In_Brand = vehicles[index].In_Brand;
            vehicle.In_Color = vehicles[index].In_Color;
            add_Vehicle.Update(add_Vehicle, vehicle);

            await add_Vehicle.ShowAsync();

            if (add_Vehicle.result == Add_Vehicle.Result.Toevoegen)
            {
                vehicles.Remove(vehicles[index]);
            }
        }




        public class Suspect
        {
            public string In_Age { get; set; }
            public string In_Length { get; set; }
            public string In_SkinColor { get; set; }
            public string In_Gender { get; set; }
        }

        private void Toevoegen(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}

