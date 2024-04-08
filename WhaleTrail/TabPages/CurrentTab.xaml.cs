﻿using System;
using System.Text.Json;
using WhaleTrail.Models;
using WhaleTrail.Services;

namespace WhaleTrail.Pages.TabPages
{
    public partial class CurrentTab : ContentPage
    {
        private DataService _dataService;

        public CurrentTab()
        {
            InitializeComponent();

            _dataService = new DataService();

            // check last fetch time before calling API
            var lastFetchTimestamp = Preferences.Get("LastAPIFetch", DateTime.MinValue);
            if (lastFetchTimestamp == DateTime.MinValue)
            {
                // default to last 30 days if DateTime.MinValue is 0001-01-01
                lastFetchTimestamp = DateTime.UtcNow.AddDays(-30);
            }

            // fetch data if more than a day since last
            if ((DateTime.UtcNow - lastFetchTimestamp) > TimeSpan.FromDays(1))
            {
                // call API & update database 
                LoadDataAsync(lastFetchTimestamp);

                // update last fetch time
                Preferences.Set("LastAPIFetch", DateTime.UtcNow);
            }
            else
            {
                // use DB
                App.SightingsRepo.GetAllSightings();
                sightingsList.ItemsSource = App.SightingsRepo.GetAllSightings();
            }
        }

        private async void LoadDataAsync(DateTime lastFetchTimestamp)
        {
            try
            {
                // call fetch function
                var data = await _dataService.FetchEncounterDataAsync(lastFetchTimestamp);

                var rootObject = JsonSerializer.Deserialize<Root>(data);

                // if there are any new results, add to DB
                if (rootObject?.results != null)
                {
                    var sightingsData = new List<SightingsData>();

                    foreach (var result in rootObject.results)
                    {
                        // Perform null checks on nested objects before accessing their properties
                        if (result.individual != null 
                            && result.individual.nickname != null
                            && result.dateRange.startDate != null 
                            && result.dateRange.startTime != null 
                            && result.location != null)
                        {
                            // add to DB
                            App.SightingsRepo.AddSighting(new Sighting
                            {
                                Name = result.individual.nickname,
                                Date = result.dateRange.startDate,
                                Time = result.dateRange.startTime,
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
    }

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

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Result
    {
        public int id { get; set; }
        public DateRange dateRange { get; set; }
        public Location location { get; set; }
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
