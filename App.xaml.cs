using System;
using wijkagent.Core.Models;
using wijkagent.Core.Services;
using wijkagent.Services;
using wijkagent.ViewModels;
using wijkagent.Views;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace wijkagent
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            UnhandledException += OnAppUnhandledException;

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
            //activate current window 
            Window.Current.Activate();
            Frame frame = Window.Current.Content as Frame; //try to get current frame
            // Handle notification activation
            if (args is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                // Get the arguments from the notification
                string notifArgs = toastActivationArgs.Argument;
                string[] parts = notifArgs.Split("_");
                if (parts[0] == "Delict")
                {
                    int delictID = int.Parse(parts[1]);
                    KaartViewModel.delict = SqlDataService.GetDelict(delictID);
                    if (frame != null)
                    {
                        frame.Navigate(typeof(DelictenPage));
                    }
                    else //sometimes the right frame isn't windows.current.content 
                    {
                        //actually gets the right frame to navigate with
                        NavigationService.Frame.Navigate(typeof(DelictenPage));
                    }
                }
            }
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.HomePage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }
    }
}
