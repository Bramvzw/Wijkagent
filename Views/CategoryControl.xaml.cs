using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using wijkagent.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace wijkagent.Views
{
    public sealed partial class CategoryControl : UserControl
    {
        public CategoryControl()
        {
            this.InitializeComponent();
        }
        public void ChangeNameField(string name)
        {
            if (name.Length > 36)
            {
                name = name.Substring(0, 33) + "...";
            }
            NameTextBlock.Text = name;
        }
        public void ChangeDescField(string description)
        {
            if (description != null)
            {
                if (description.Length > 30)
                {
                    description = description.Substring(0, 27) + "...";
                }

                DescTextBlock.Text = description;
            }
        }
    }
}
