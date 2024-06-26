﻿using GeoCoordinatePortable;
using System.Globalization;
using System.Text.Json;
using WhaleTrail.Models;
using WhaleTrail.Services;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Maui = Microsoft.Maui.Devices.Sensors;

namespace WhaleTrail.Pages.TabPages
{
    public partial class CurrentTab : ContentPage
    {
        private DataService _dataService;
        private double phoneLat;
        private double phoneLng;

        public CurrentTab()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            _dataService = new DataService();
            await GetLocationAsync();

            // check last fetch time before calling API
            var lastFetchTimestamp = Preferences.Get("LastAPIFetch", DateTime.MinValue);
            Console.WriteLine($"lastFetchTimestamp {lastFetchTimestamp}");
            
            if (lastFetchTimestamp == DateTime.MinValue)
            {
                // default to last 60 days if DateTime.MinValue is 0001-01-01
                lastFetchTimestamp = DateTime.UtcNow.AddDays(-60);
                Console.WriteLine($"SET lastFetchTimestamp {lastFetchTimestamp}");
            }

            int count = App.SightingsRepo.GetAllSightings().Count;

            // fetch data if more than a day since last
            if (count == 0)
            {
                Console.WriteLine("NEW DB");
                // default to last 60 days for new databases
                LoadDataAsync(DateTime.UtcNow.AddDays(-60), phoneLat, phoneLng);

                // update last fetch time
                Preferences.Set("LastAPIFetch", DateTime.UtcNow);
            }
            else if ((DateTime.UtcNow - lastFetchTimestamp) > TimeSpan.FromDays(1))
            {
                Console.WriteLine("EXISTING DB");
                Console.WriteLine($"EDB lastFetchTimestamp {lastFetchTimestamp}");
                Console.WriteLine($"EDB phoneLat {phoneLat}");
                Console.WriteLine($"EDB phoneLng {phoneLng}");
                LoadDataAsync(lastFetchTimestamp, phoneLat, phoneLng);

                // update last fetch time
                Preferences.Set("LastAPIFetch", DateTime.UtcNow);
            }
            else
            {
                Console.WriteLine("RECENTLY UPDATED");
                // use DB
                App.SightingsRepo.GetAllSightings();
                sightingsList.ItemsSource = App.SightingsRepo.GetAllSightings();
            }
        }

        private async Task GetLocationAsync()
        {
            Console.WriteLine("Checking location permissions...");
            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (permissionStatus != PermissionStatus.Granted)
            {
                Console.WriteLine("Requesting location permissions...");
                permissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (permissionStatus != PermissionStatus.Granted)
                {
                    Console.WriteLine("Location permission denied.");
                    // setting default GPS location if user denies
                    phoneLat = 36.645;
                    phoneLng = -121.872;

                    return;
                }
            }

            try
            {
                var location = await Geolocation.GetLocationAsync();
                Console.WriteLine(location);

                if (location != null)
                {
                    phoneLat = location.Latitude;
                    phoneLng = location.Longitude;
                    CenterMap(phoneLat, phoneLng);

                    Console.WriteLine($"Latitude: {phoneLat}, Longitude: {phoneLng}");
                }
                else
                {
                    Console.WriteLine("No location available.");
                    // setting default GPS location cant be found
                    phoneLat = 36.645;
                    phoneLng = -121.872;
                    CenterMap(phoneLat, phoneLng);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                // setting default GPS location cant be found
                phoneLat = 36.645;
                phoneLng = -121.872;
                CenterMap(phoneLat, phoneLng);
            }
        }

        private void CenterMap(double latitude, double longitude)
        {
            Console.WriteLine("CenterMap");
            Console.WriteLine($"CM phoneLat {latitude}");
            Console.WriteLine($"CM phoneLng {longitude}");

            Maui.Location gpsPos = new Maui.Location(latitude, longitude);
            MapSpan mapSpan = new MapSpan(gpsPos, 0.3, 0.6);

            map.MoveToRegion(mapSpan);

            SetMapPins();

            Pin pin = new Pin
            {
                Label = "Current Location",
                Address = "you are here",
                Type = PinType.Place,
                Location = gpsPos
            };
            map.Pins.Add(pin);
        }

        private async void LoadDataAsync(DateTime lastFetchTimestamp, double lat, double lng)
        {
            Console.WriteLine("LoadDataAsync");
            Console.WriteLine($"lastFetchTimestamp: {lastFetchTimestamp}");
            // call API, fetch data & update database 
            try
            {
                // call fetch function
                var data = await _dataService.FetchEncounterDataAsync(lastFetchTimestamp, lat, lng);

                var rootObject = JsonSerializer.Deserialize<Root>(data);

                // if there are any new results, add to DB
                if (rootObject?.results != null)
                {
                    var sightingsData = new List<SightingsData>();

                    foreach (var result in rootObject.results)
                    {
                        // Perform null checks on nested objects before accessing their properties
                        if (result.individual != null
                            && !string.IsNullOrWhiteSpace(result.individual.nickname)
                            && result.dateRange != null
                            && !string.IsNullOrWhiteSpace(result.dateRange.startDate)
                            && !string.IsNullOrWhiteSpace(result.dateRange.startTime)
                            && result.location != null)
                        {
                            // create date object
                            // Parse the date and time strings into DateTime objects
                            DateTime date = DateTime.ParseExact(result.dateRange.startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            DateTime time = DateTime.ParseExact(result.dateRange.startTime, "HH:mm:ss", CultureInfo.InvariantCulture);
                            // Combine the date and time into a single DateTime
                            DateTime sightingDateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);

                            // add to DB
                            App.SightingsRepo.AddSighting(new Sighting
                            {
                                Id = result.individual.id,
                                Name = result.individual.nickname,
                                Date = sightingDateTime.ToString("MM-dd-yy HH:mm"),
                                Lat = result.location.lat,
                                Long = result.location.lng
                            });
                        }
                    }
                }
                
                // use DB data
                App.SightingsRepo.GetAllSightings();
                sightingsList.ItemsSource = App.SightingsRepo.GetAllSightings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                // use current database without updating
                App.SightingsRepo.GetAllSightings();
                sightingsList.ItemsSource = App.SightingsRepo.GetAllSightings();
            }
        }
        // TODO: show whale info on click
        private void SetMapPins()
        {
            var sightings = App.SightingsRepo.GetAllSightings(); 
            foreach (var sighting in sightings)
            {
                var pin = new Pin
                {
                    Label = sighting.Name, 
                    Type = PinType.Place,
                    Location = new Maui.Location(sighting.Lat, sighting.Long) 
                };
                pin.MarkerClicked += async (sender, e) =>
                {
                    e.HideInfoWindow = true;
                    string pinName = ((Pin)sender).Label;
                    await DisplayAlert("Whale: " + pinName, "Location: : Lat " + sighting.Lat + " Lng " + sighting.Long, "Close");
                };

                map.Pins.Add(pin);
            }
        }

        // FILTER FUNCTIONS
        private void OnSortByNameClicked(object sender, EventArgs e)
        {
            var sortedByName = App.SightingsRepo.GetAllSightings().OrderBy(s => s.Name).ToList();
            sightingsList.ItemsSource = sortedByName;
        }

        private void OnSortByDateClicked(object sender, EventArgs e)
        {
            var sortedByDate = App.SightingsRepo.GetAllSightings()
                                                .Select(s => new
                                                {
                                                    Sighting = s,
                                                    ParsedDate = DateTime.ParseExact(s.Date, "MM-dd-yy HH:mm", CultureInfo.InvariantCulture)
                                                })
                                                .OrderByDescending(x => x.ParsedDate)
                                                .Select(x => x.Sighting)
                                                .ToList();
            sightingsList.ItemsSource = sortedByDate;
        }

        private void OnSortByDistanceClicked(object sender, EventArgs e)
        {
            var sortedByDistance = App.SightingsRepo.GetAllSightings().OrderBy(s =>
                CalculateDistance(s.Lat, s.Long)
            ).ToList();

            sightingsList.ItemsSource = sortedByDistance;
        }

        private double CalculateDistance(double lat, double lng)
        {
            // Center point coordinates
            // currently somewhere in Monterey CA until using phone GPS
            double centerLat = 36.0;
            double centerLong = -121.0;

            // using GeoCoordinate.NetCore package
            var sCoord = new GeoCoordinate(centerLat, centerLong);
            var eCoord = new GeoCoordinate(lat, lng);

            return sCoord.GetDistanceTo(eCoord);
        }

        // SIGHTING MODAL SHOW / HIDE
        private void ShowModal(object sender, EventArgs e)
        {
            modalScrollView.IsVisible = true;
        }
        
        private void CloseModal(object sender, EventArgs e)
        {
            modalScrollView.IsVisible = false;
        }
    }

    // UTILITY CLASSES
    public class SightingsData
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
    }

    public class Avatar
    {
        public object id { get; set; }
        public string type { get; set; }
        public string thumbUrl { get; set; }
        public string url { get; set; }
    }

    public class DateRange
    {
        public string startDate { get; set; }
        public string startTime { get; set; }
        public object endDate { get; set; }
        public string endTime { get; set; }
        public string timezone { get; set; }
    }

    public class DisplayImage
    {
        public int id { get; set; }
        public string type { get; set; }
        public string thumbUrl { get; set; }
        public string url { get; set; }
    }

    public class Individual
    {
        public int id { get; set; }
        public string species { get; set; }
        public string alternateId { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public Avatar avatar { get; set; }
    }

    public class GeoLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Result
    {
        public int id { get; set; }
        public DateRange dateRange { get; set; }
        public GeoLocation location { get; set; }
        public string region { get; set; }
        public Individual individual { get; set; }
        public string species { get; set; }
        public int minCount { get; set; }
        public int maxCount { get; set; }
        public DisplayImage displayImage { get; set; }
        public object attrs { get; set; }
        public bool approved { get; set; }
        public List<object> orgIds { get; set; }
        public bool @public { get; set; }
    }

    public class Root
    {
        public List<Result> results { get; set; }
        public bool limitExceeded { get; set; }
    }

}
