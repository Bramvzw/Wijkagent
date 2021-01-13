using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wijkagent.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace wijkagent.Views
{
    public sealed partial class CategoriesPage : Page
    {
        public CategoriesViewModel ViewModel { get; } = new CategoriesViewModel();
        public CategoriesPage()
        {
            InitializeComponent();
        }
        public void AddToCategoriesGrid(UserControl userControl, int rowNumber)
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(95);
            CategoriesGrid.RowDefinitions.Add(row);
            CategoriesGrid.Children.Add(userControl);
            Grid.SetRow(userControl, rowNumber);
        }
        public void EmptyCategoriesGrid()
        {
            CategoriesGrid.Children.Clear();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(ViewModel.categoriesPage == this))
            {
                ViewModel.categoriesPage = this;
            }
            await ViewModel.InitializeAsync();
        }

        private async void Delete_click(object sender, RoutedEventArgs e)
        {

            ContentDialog CD = new ContentDialog();
            CD.Title += "Categorie verwijderen";
            CD.Content += "Weet u zeker dat u deze categorie wil verwijderen? Delicten van deze categorie worden overgezet naar categorie 'Overig'.";
            CD.PrimaryButtonText += "Ja";
            CD.SecondaryButtonText += "Nee";

            ContentDialogResult result = await CD.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.Delete();
            }
        }

        private async void Add_click(object sender, RoutedEventArgs e)
        {
            ViewModel.Add();
        }
    }
}
