using System;
using System.Collections.Generic;
using System.Linq;
using wijkagent.Core.Models;
using wijkagent.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using wijkagent.Core.Services;
using System.Collections.ObjectModel;

namespace wijkagent.Views
{
    public sealed partial class DelictenPage : Page
    {
        public DelictenViewModel ViewModel { get; } = new DelictenViewModel();

        public DelictenPage()
        {
            InitializeComponent();
            Loaded += DelictenPage_Loaded;
        }

        private void DelictenPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadData(MasterDetailsViewControl.ViewState);
            //if the user clicked on a delict on the map, the static delict of kaartviewmodel is that delict, if the user didnt, the static delict is null
            //if the user clicked on a notification KaartViewModel.delict also has the delict of the notif that was clicked on
            if (KaartViewModel.delict != null)
            {
                IEnumerable<Delict> result = (from item in ViewModel.Delicts
                                              where item.Id == KaartViewModel.delict.Id
                                              select item);
                ViewModel.Selected = result.First();
                //set delict on kaartviewmodel back to null
                KaartViewModel.delict = null;
            }
            FillFilterComboBox();
            FilterComboBox.DropDownClosed += OnTextSubmittedEvent;
        }
        private void FillFilterComboBox()
        {
            //add an "alle delicten" options to the filtercombobox 
            FilterComboBox.Items.Add("Alle delicten");
            foreach (Category category in SqlDataService.AllCategories()) //add all categories as options to filter
                FilterComboBox.Items.Add(category.Name);

        }
        public void OnTextSubmittedEvent(Object o, Object anotherObject)
        {
            Category pickedCategory = null; //it's null if "Alle Delicten" has been selected, as it is not a category
            string selected = (string)FilterComboBox.SelectedItem; //get selected item

            if (!(selected == null) && !(selected.Equals("Alle delicten"))) //if false: pickedCategory will stay null
            {
                foreach (Category category in SqlDataService.AllCategories())
                {
                    if (category.Name.Equals(selected))
                    {
                        pickedCategory = category; //get the picked category
                    }
                }

            }
            ViewModel.Delicts.Clear(); //clear the delicts, it will be filled in the foreach loop
            foreach (Delict delict in SqlDataService.AllDelicts())
            {
                if (pickedCategory == null || delict.CategoryId == pickedCategory.Id) //add delict if delict is from picked category OR if pickedcategory is null (all delicts will be added)
                {
                    ViewModel.Delicts.Add(delict);
                }
            }
            MasterDetailsViewControl.UpdateLayout();
        }
    }
}
