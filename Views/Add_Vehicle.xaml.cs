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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace wijkagent.Views
{
    public sealed partial class Add_Vehicle : ContentDialog
    {
        public Result result;
        public static ObservableCollection<Vehicle> Vehicles = new ObservableCollection<Vehicle>();


        public Add_Vehicle()
        {
            this.InitializeComponent();
        }

        private void Toevoegen(object sender, RoutedEventArgs e)
        {
            Vehicle vehicle = new Vehicle();
            if (In_Plate.Text != "")
            {
                vehicle.In_Plate = In_Plate.Text;
            }
            else
            {
                vehicle.In_Plate = "onbekend";
            }
            if (In_Color.Text != "")
            {
                vehicle.In_Color = In_Color.Text;
            }
            else
            {
                vehicle.In_Color = "onbekend";
            }
            if (In_Brand.Text != "")
            {
                vehicle.In_Brand = In_Brand.Text;
            }
            else
            {
                vehicle.In_Brand = "onbekend";
            }
            Vehicles.Add(vehicle);
            this.Hide();

        }

        public void Update(Add_Vehicle add_Vehicle, Vehicle vehicle)
        {
            add_Vehicle.In_Plate.Text = vehicle.In_Plate;
            add_Vehicle.In_Brand.Text = vehicle.In_Brand;
            add_Vehicle.In_Color.Text = vehicle.In_Color;
        }

        public enum Result
        {
            Toevoegen,
            Annuleren
        }

        public class Vehicle
        {
            public string In_Plate { get; set; }
            public string In_Color { get; set; }
            public string In_Brand { get; set; }
        }

        private void Annuleren(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
