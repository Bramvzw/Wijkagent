using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using wijkagent.Core.Services;
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
    public sealed partial class Add_Suspect : ContentDialog
    {

        public static ObservableCollection<Suspect> Suspects = new ObservableCollection<Suspect>();
        public Result result;

        public Add_Suspect()
        {
            this.InitializeComponent();
        }

        private void Annuleren(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private async void Toevoegen(object sender, RoutedEventArgs e)
        {
            Suspect suspect = new Suspect();
            if (In_Age.Text != "")
            {
                suspect.In_Age = In_Age.Text;

            }
            else
            {
                suspect.In_Age = "onbekend";
            }

            if (In_Length.Text != "")
            {
                suspect.In_Length = In_Length.Text;

            }
            else
            {
                suspect.In_Length = "onbekend";
            }

            if (In_SkinColor.Text != "")
            {
                suspect.In_SkinColor = In_SkinColor.Text;

            }
            else
            {
                suspect.In_SkinColor = "onbekend";
            }
            if ((ComboBoxItem)In_Gender.SelectedItem != null)
            {
                ComboBoxItem typeItem = (ComboBoxItem)In_Gender.SelectedItem;
                suspect.In_Gender = typeItem.Content.ToString();
            }
            else
            {
                suspect.In_Gender = "onbekend";
            }
            Suspects.Add(suspect);

            this.result = Result.Toevoegen;
            this.Hide();
        }

        public void Update(Add_Suspect add_Suspect, Suspect suspect)
        {
            add_Suspect.In_Age.Text = suspect.In_Age;
            add_Suspect.In_Length.Text = suspect.In_Length;
            add_Suspect.In_SkinColor.Text = suspect.In_SkinColor;
            add_Suspect.In_Gender.Text = suspect.In_Gender;


            //SqlDataService.RemoveDespriptionSuspect();
        }

        public enum Result
        {
            Toevoegen,
            Annuleren
        }


        public class Suspect
        {
            public string In_Age { get; set; }
            public string In_Length { get; set; }
            public string In_SkinColor { get; set; }
            public string In_Gender { get; set; }
        }
    }
}
