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
    public sealed partial class Add_Time : ContentDialog
    {
        public static ObservableCollection<Time_Loc> Data = new ObservableCollection<Time_Loc>();
        public Result result;
        public Add_Time()
        {
            this.InitializeComponent();
        }

        private void Toevoegen(object sender, RoutedEventArgs e)
        {
            Time_Loc time = new Time_Loc();

            if (In_Location.Text != "")
            {
                time.In_Location = In_Location.Text;
            }
            else
            {
                time.In_Location = "onbekend";
            }

            if (In_Date.SelectedDate != null && In_Time.SelectedTime != null)
            {
                time.In_Location = In_Location.Text;
                var date = this.In_Date.Date;
                time.In_Date = date.DateTime;
                var timeonly = In_Time.Time;
                Console.WriteLine(timeonly);
                time.In_Time = timeonly;



            }
            else
            {
                time.In_Date = DateTime.Parse("00:00");
            }
            Data.Add(time);
            this.result = Result.Toevoegen;
            this.Hide();
        }

        public void Update(Add_Time add_Time, Time_Loc time_Loc)
        {
            add_Time.In_Time.SelectedTime = time_Loc.In_Time;
            add_Time.In_Date.SelectedDate = time_Loc.In_Date;
            add_Time.In_Location.Text = time_Loc.In_Location;

        }

        public enum Result
        {
            Toevoegen,
            Annuleren
        }

        public class Time_Loc
        {
            public string In_Location { get; set; }
            public TimeSpan In_Time { get; set; }
            public DateTime In_Date { get; set; }
        }

        private void Annuleren(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
