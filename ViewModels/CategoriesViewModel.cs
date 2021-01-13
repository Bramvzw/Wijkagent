using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wijkagent.Core.Models;
using Windows.UI.Xaml.Controls;
using wijkagent.Views;
using Windows.UI.Xaml.Media;
using wijkagent.Core.Services;
using Windows.UI.Xaml;
using Windows.UI;

namespace wijkagent.ViewModels
{
    public class CategoriesViewModel
    {
        public CategoriesPage categoriesPage;
        private int? idSelected = null;
        public CategoriesViewModel()
        {
            AddCategoriesToPage(); //add categories to page using CategoryControl
        }
        public async Task InitializeAsync()
        {
            AddCategoriesToPage();
        }
        public void AddCategoriesToPage()
        {
            if (categoriesPage != null)
            {
                categoriesPage.EmptyCategoriesGrid();
                //to keep track of which row it should be put into
                int rowNumber = 0;
                //empty the grid, so there will not be any duplicates
                //categoriesPage.EmptyCategoriesGrid();
                //filling the grid
                foreach (Category category in SqlDataService.AllCategories())
                {
                    //make a category control for each existing category + give each field the right text
                    CategoryControl categoryControl = new CategoryControl();
                    categoryControl.ChangeNameField(category.Name);
                    categoryControl.ChangeDescField(category.Description);
                    //the name of the categorycontrol is the ID, so it's easy to find out which category the user clicked on
                    categoryControl.Name = category.Id.ToString();
                    //setting the size
                    categoryControl.Height = 75;
                    categoryControl.Width = 400;
                    //make the button
                    Button btn = categoryControl.FindName("SelectButton") as Button;
                    btn.Click += ClickButtonCategory;
                    btn.Name = categoryControl.Name;
                    //add control to grid
                    categoriesPage.AddToCategoriesGrid(categoryControl, rowNumber);
                    rowNumber++;
                }
            }

        }
        /* 
          When a category is clicked, the category gets another color to indicate being selected
          The previous selected category returns to normal color
          The Selected variable gets updated to the clicked category
        */
        public void ClickButtonCategory(object sender, RoutedEventArgs e)
        {
            if (idSelected != null)
            {
                CategoryControl LastCategory = categoriesPage.FindName(idSelected.ToString()) as CategoryControl;
                Grid LastGrid = LastCategory.FindName("Grid") as Grid;
                LastGrid.Background = new SolidColorBrush(Color.FromArgb(255, 198, 198, 198));
            }
            else
            {
                Button btn = categoriesPage.FindName("Deletebutton") as Button;
                btn.Visibility = Visibility.Visible;
            }

            Button ButtonCategory = sender as Button;
            idSelected = Int32.Parse(ButtonCategory.Name);

            CategoryControl category = categoriesPage.FindName(idSelected.ToString()) as CategoryControl;
            Grid Grid = category.FindName("Grid") as Grid;
            Grid.Background = new SolidColorBrush(Color.FromArgb(255, 132, 132, 132));
            SelectedChanged();
        }

        /*
         The category details on the bottom of the page get updated to the correct values
         If no category is selected or the selected category is the "overig" category, the delete button disappears
        */
        public void SelectedChanged()
        {
            Button btn = categoriesPage.FindName("Deletebutton") as Button;
            if (idSelected == 0 || idSelected == null)
            {
                btn.Visibility = Visibility.Collapsed;

            }
            else
            {
                btn.Visibility = Visibility.Visible;

            }
            Category current_category = SqlDataService.GetCategory(idSelected.GetValueOrDefault());
            int delict_amount = SqlDataService.GetCatAmount(current_category.Id);

            TextBlock categoryName = categoriesPage.FindName("CategoryName") as TextBlock;
            TextBlock categoryAmount = categoriesPage.FindName("CategoryAmount") as TextBlock;
            TextBlock CategoryDescription = categoriesPage.FindName("CategoryDescription") as TextBlock;

            if (idSelected == null)
            {
                categoryName.Text = "Selecteer een categorie";
                categoryAmount.Text = "Selecteer een categorie";
                CategoryDescription.Text = "Selecteer een categorie";
            }
            else
            {
                categoryName.Text = current_category.Name;
                categoryAmount.Text = delict_amount.ToString();
                if (!String.IsNullOrEmpty(current_category.Description))
                {
                    CategoryDescription.Text = current_category.Description;
                }
                else
                {
                    CategoryDescription.Text = "Geen beschrijving";
                }
            }
        }

        //Deletes the selected category from the database, after moving all of its delicts to category 0
        public void Delete()
        {
            if (SqlDataService.GetCatAmount(idSelected.GetValueOrDefault()) > 0)
            {
                SqlDataService.CleanCategory(idSelected.GetValueOrDefault());
            }
            SqlDataService.RemoveCategory(idSelected.GetValueOrDefault());
            Reload();

        }

        //Adds a category to the database
        //Checks if all necessary values are filled
        public async void Add()
        {
            TextBox NewCatName = categoriesPage.FindName("AddCatName") as TextBox;
            TextBox NewCatDesc = categoriesPage.FindName("AddCatDesc") as TextBox;

            if (NewCatName.Text == "")
            {
                ContentDialog CD = new ContentDialog();
                CD.Title += "Categorie toevoegen";
                CD.Content += "Vul een categorie naam in";
                CD.PrimaryButtonText += "Ok";

                await CD.ShowAsync();
            }
            else
            {
                ContentDialog CD = new ContentDialog();
                CD.Title += "Categorie toevoegen";
                CD.Content += "Weet u zeker dat u deze categorie wil toevoegen?";
                CD.PrimaryButtonText += "Ja";
                CD.SecondaryButtonText += "Nee";

                ContentDialogResult result = await CD.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    SqlDataService.AddCategory(NewCatName.Text, NewCatDesc.Text);
                    Reload();
                }
                NewCatName.Text = string.Empty;
                NewCatDesc.Text = string.Empty;

            }
        }

        //refresh the category list, as well as update the selected category information
        private void Reload()
        {
            idSelected = null;
            AddCategoriesToPage();
            SelectedChanged();
        }



    }
}
