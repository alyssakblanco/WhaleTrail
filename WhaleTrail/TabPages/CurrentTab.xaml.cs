using System;
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

            // TODO: add new fucntion to access from DB instead
            // LoadDataAsync();
            App.SightingsRepo.GetAllSightings();

            sightingsList.ItemsSource = App.SightingsRepo.GetAllSightings();
        }

        private async void LoadDataAsync()
        {
            Console.WriteLine("LoadDataAsync");
            Console.WriteLine("LoadDataAsync");

            try
            {
                Console.WriteLine("TRY");
                Console.WriteLine("TRY");

                var data = await _dataService.FetchEncounterDataAsync();
                Console.WriteLine("DATA");
                Console.WriteLine(data);

                var rootObject = JsonSerializer.Deserialize<Root>(data);
                Console.WriteLine("rootObject");
                Console.WriteLine(rootObject);

                if (rootObject?.results != null)
                {
                    Console.WriteLine("not null");
                    Console.WriteLine("not null");

                    var sightingsData = new List<SightingsData>();

                    foreach (var result in rootObject.results)
                    {
                        Console.WriteLine("processing");
                        Console.WriteLine("result");
                        Console.WriteLine(result);
                        

                        // Perform null checks on nested objects before accessing their properties
                        if (result.individual != null 
                            && result.individual.nickname != null
                            && result.dateRange.startDate != null 
                            && result.dateRange.startTime != null 
                            && result.location != null)
                        {
                            Console.WriteLine("result.individual.nickname??");
                            Console.WriteLine(result.individual.nickname);
                            Console.WriteLine("result.dateRange.startDate");
                            Console.WriteLine(result.dateRange.startDate);
                            Console.WriteLine("result.dateRange.startTime");
                            Console.WriteLine(result.dateRange.startTime);
                            Console.WriteLine("result.location");
                            Console.WriteLine(result.location);

                            sightingsData.Add(new SightingsData
                             {
                                 Name = result.individual.nickname,
                                 Date = result.dateRange.startDate,
                                 Time = result.dateRange.startTime,
                                 Lat = result.location.lat,
                                 Long = result.location.lng
                             });

                            App.SightingsRepo.AddSighting(new Sighting
                            {
                                Name = result.individual.nickname,
                                Date = result.dateRange.startDate,
                                Time = result.dateRange.startTime,
                                Lat = result.location.lat,
                                Long = result.location.lng
                            });
                        }
                        else
                        {
                            Console.WriteLine("ELSE");
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
                Console.WriteLine("CATCH");
                Console.WriteLine("CATCH");
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
