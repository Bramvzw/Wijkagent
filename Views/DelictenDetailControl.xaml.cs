using System;
using System.Diagnostics;
using wijkagent.Core.Models;
using wijkagent.Core.Services;
using wijkagent.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace wijkagent.Views
{
    public sealed partial class DelictenDetailControl : UserControl
    {
        public Delict MasterMenuItem
        {
            get
            {
                Delict Delict = GetValue(MasterMenuItemProperty) as Delict;
                if (Delict != null)
                    ViewModel.UpdateTweets(Delict.Id);
                return Delict;
            }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public DelictenViewModel ViewModel { get; } = new DelictenViewModel();

        public static readonly DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(Delict), typeof(DelictenDetailControl), new PropertyMetadata(null, OnMasterMenuItemPropertyChanged));

        public DelictenDetailControl()
        {
            InitializeComponent();
            this.ViewModel = new DelictenViewModel();
        }

        private static void OnMasterMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DelictenDetailControl;

            Debug.WriteLine("MASTER MENU SWITCHED");
        }


        // Change status of delict
        private void ChangeStatus(object sender, RoutedEventArgs e)
        {
            if (MasterMenuItem.Active)
            {
                SqlDataService.SetInactive(MasterMenuItem.Id);
                MasterMenuItem.Active = false;
                StatusDelict.Content = "False";

            }
            else
            {
                SqlDataService.SetActive(MasterMenuItem.Id);
                Debug.WriteLine("1");
                MasterMenuItem.Active = true;
                StatusDelict.Content = "True";
            }
        }
    }
}
