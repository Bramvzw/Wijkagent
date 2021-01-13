using System;
using wijkagent.Core.Models;
using wijkagent.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace wijkagent.Views
{
    public sealed partial class KaartPage : Page
    {
        public KaartViewModel ViewModel { get; } = new KaartViewModel();

        public KaartPage()
        {
            InitializeComponent();
            //putting the right ammount of delicts on the screen
            //note: this has to be changed some time
            //DelictAmmountText.Text = "0"; //for now
            //logoutButton.AddHandler has to be done for the button to work
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.kaartPage = this;
            await ViewModel.InitializeAsync(mapControl);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Cleanup();
        }

        public void ShowDelictDetails()
        {
            Frame.Navigate(typeof(DelictenPage));
        }
    }
}
