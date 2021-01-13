using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using wijkagent.Core.Models;
using wijkagent.Core.Services;
using wijkagent.Helpers;
using wijkagent.Services;
using wijkagent.Views;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace wijkagent.ViewModels
{
    public class KaartViewModel : Observable
    {
        public static Delict delict;
        // TODO WTS: Set your preferred default zoom level
        private const double DefaultZoomLevel = 12;

        private readonly LocationService _locationService;

        // TODO WTS: Set your preferred default location if a geolock can't be found.
        private readonly BasicGeoposition _defaultPosition = new BasicGeoposition()
        {
            Latitude = 52.528164,
            Longitude = 6.1095534
        };

        private double _zoomLevel;

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set { Set(ref _zoomLevel, value); }
        }

        private Geopoint _center;

        public Geopoint Center
        {
            get { return _center; }
            set { Set(ref _center, value); }
        }
        public KaartPage kaartPage;
        public KaartViewModel()
        {
            _locationService = new LocationService();
            Center = new Geopoint(_defaultPosition);
            ZoomLevel = DefaultZoomLevel;
        }

        public async Task InitializeAsync(MapControl map)
        {
            delict = null;
            if (_locationService != null)
            {
                _locationService.PositionChanged += LocationService_PositionChanged;

                var initializationSuccessful = await _locationService.InitializeAsync();

                if (initializationSuccessful)
                {
                    await _locationService.StartListeningAsync();
                }

                if (initializationSuccessful && _locationService.CurrentPosition != null)
                {
                    Center = _locationService.CurrentPosition.Coordinate.Point;
                }
                else
                {
                    Center = new Geopoint(_defaultPosition);
                }
            }

            if (map != null)
            {
                foreach (Delict Delict in SqlDataService.GetAllActiveDelicts())
                {
                    AddMapIcon(map, new Geopoint(new BasicGeoposition()
                    {
                        Latitude = (double)Delict.Latitude,
                        Longitude = (double)Delict.Longitude
                    }), Delict.Name, Delict.Id);
                }
                map.MapElementClick += OnMapElementClickEvent;
            }
        }

        private void LocationService_PositionChanged(object sender, Geoposition geoposition)
        {
            if (geoposition != null)
            {
                Center = geoposition.Coordinate.Point;
            }
        }

        private void AddMapIcon(MapControl map, Geopoint position, string title, int id)
        {
            var mapIcon = new MapIcon()
            {
                Location = position,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Title = title,
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/map.png")),
                ZIndex = 0,
                Tag = id
            };
            map.MapElements.Add(mapIcon);
        }
        private void OnMapElementClickEvent(object o, MapElementClickEventArgs eventArgs)
        {
            //for when a mapelement is clicked
            //getting all mapicons near the click
            IEnumerable<MapElement> mapIconsNearClick = (from item in eventArgs.MapElements
                                                         where item is MapIcon
                                                         select item);
            double latCLick = eventArgs.Location.Position.Latitude;
            double longClick = eventArgs.Location.Position.Longitude;
            Point positionClicked = eventArgs.Position;
            DelictenDetailControl detailControl = new DelictenDetailControl();
            if (mapIconsNearClick.Count() == 1)
            {
                MapIcon m = (MapIcon)mapIconsNearClick.First();
                Geopoint iconPoint = m.Location;
                //show the delict info
                kaartPage.ShowDelictDetails();
                delict = SqlDataService.GetDelict((Int32)m.Tag);
            }
            else if (mapIconsNearClick.Count() > 1)
            {
                //lowest difference is 100 because it needs a value and 100 is too much of a difference anyway
                double latLowestDifference = 100;
                double longLowestDifference = 100;
                MapIcon closestMapIcon = null;
                //pick mapicon closest to the click
                foreach (MapIcon mapIcon in mapIconsNearClick)
                {
                    double latIcon = mapIcon.Location.Position.Latitude;
                    double longIcon = mapIcon.Location.Position.Longitude;
                    if (latCLick - latIcon < latLowestDifference && longClick - longIcon < longLowestDifference)
                    {
                        closestMapIcon = mapIcon;
                        latLowestDifference = latCLick - latIcon;
                        longLowestDifference = longClick - longIcon;
                    }
                }
                if (closestMapIcon != null)
                {
                    //show the delict of the closest mapicon 
                    kaartPage.ShowDelictDetails();
                    delict = SqlDataService.GetDelict((Int32)closestMapIcon.Tag);
                }
            }
        }

        public void Cleanup()
        {
            if (_locationService != null)
            {
                _locationService.PositionChanged -= LocationService_PositionChanged;
                _locationService.StopListening();
            }
        }
    }
}
