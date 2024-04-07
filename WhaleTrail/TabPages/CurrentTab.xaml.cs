using System;
using System.Text.Json;
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
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var data = await _dataService.FetchEncounterDataAsync();
                // Console.WriteLine(data);
                var rootObject = JsonSerializer.Deserialize<Root>(data);
                // Console.WriteLine("rootObject");
                // Console.WriteLine(rootObject);

                if (rootObject?.results != null)
                {
                    var sightingsData = new List<SightingsData>();

                    foreach (var result in rootObject.results)
                    {
                        // Perform null checks on nested objects before accessing their properties
                        if (result.individual.nickname != null && result.dateRange.startDate != null && result.location != null)
                        {
                            sightingsData.Add(new SightingsData
                            {
                                Name = result.individual.nickname,
                                Date = result.dateRange.startDate,
                                Time = result.dateRange.startTime,
                                Lat = result.location.lat,
                                Long = result.location.lng
                            });
                        }
                    }

                    Console.WriteLine("sightingsData");
                    Console.WriteLine(sightingsData);
                    foreach (var sighting in sightingsData)
                    {
                        Console.WriteLine($"Name: {sighting.Name}, Date: {sighting.Date}, Time: {sighting.Time}, Lat: {sighting.Lat}, Long: {sighting.Long}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
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
