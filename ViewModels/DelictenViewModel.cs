using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;

using wijkagent.Core.Models;
using wijkagent.Core.Services;
using wijkagent.Helpers;
using wijkagent.Views;
using Windows.UI.Xaml.Controls;

namespace wijkagent.ViewModels
{
    public class DelictenViewModel : Observable
    {
        private Delict _selected;

        public Delict Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }



        public ObservableCollection<Delict> Delicts { get; private set; } = new ObservableCollection<Delict>();
        public ObservableCollection<Tweet> Tweets { get; set; } = new ObservableCollection<Tweet>();

        public DelictenViewModel()
        {
        }

        public void UpdateTweets(int Id)
        {
            Tweets.Clear();
            foreach (Tweet Tweet in SqlDataService.GetTweets(Id))
            {
                Tweets.Add(Tweet);
            }
        }


        public Tweet DefaultTweet { get; set; } = new Tweet();



        public void LoadData(MasterDetailsViewState viewState)
        {
            Delicts.Clear();

            var data = SqlDataService.AllDelicts();

            foreach (var item in data)
            {
                Delicts.Add(item);
            }

            if (viewState == MasterDetailsViewState.Both)
            {
                Selected = Delicts.First();
            }

        }
    }
}
