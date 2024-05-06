using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Text.Json;
using WhaleTrail.Models;
using WhaleTrail.Services;

namespace WhaleTrail
{
    public partial class WhalesPage : ContentPage
    {
        private DataService _dataService;

        public WhalesPage()
        {
            InitializeComponent();

            _dataService = new DataService();
            AddWhaleInfo();
        }

        private async void AddWhaleInfo()
        {
            int[] idsArray = GetSightingIds();

            foreach (int id in idsArray)
            {
                if (App.WhaleInfoRepo.IsIdInDatabase(id))
                {
                    Console.WriteLine("Already in database");
                }
                else
                {
                    string whaleId = id.ToString();
                    
                    try
                    {
                        var data = await _dataService.FetchWhaleDataAsync(whaleId);
                        var rootObject = JsonSerializer.Deserialize<Root>(data);
                       // Console.WriteLine($"data!!!: {data}");
                       // Console.WriteLine(rootObject);

                        string nickname;
                        string species;
                        string sex;
                        string bio;


                        if (rootObject.individual.nickname == null)
                        {
                            nickname = "Nameless";
                        }
                        else
                        {
                            nickname = rootObject.individual.nickname.ToString();
                        }

                        if (rootObject.individual.species == null)
                        {
                            species = "Whale";
                        }
                        else
                        {
                            species = rootObject.individual.species;
                        }

                        if (rootObject.individual.sex == null)
                        {
                            sex = "N/A";
                        }
                        else
                        {
                            sex = rootObject.individual.sex.ToString();
                        }

                        if (rootObject.bio == null)
                        {
                            bio = "No info :(";
                        }
                        else
                        {
                            bio = rootObject.bio.ToString();
                        }

                        Console.WriteLine($"id: {rootObject.individual.id}");
                        Console.WriteLine($"name: {nickname}");
                        Console.WriteLine($"species: {species}");
                        Console.WriteLine($"sex: {sex}");
                        Console.WriteLine($"Img: {rootObject.individual.avatar.url}");
                        Console.WriteLine($"bio: {bio}");

                        App.WhaleInfoRepo.AddWhale(new WhaleInfo
                        {
                            Id = rootObject.individual.id,
                            Name = nickname,
                            Species = species,
                            Sex = sex,
                            Img = rootObject.individual.avatar.url,
                            Bio = bio
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }

            CreateInfoCards();
        }

        private int[] GetSightingIds()
        {
            // Assuming App.SightingsRepo.All() returns an IEnumerable<Sighting>
            var sightings = App.SightingsRepo.GetAllSightings();

            // Create a list to store the Ids
            List<int> ids = new List<int>();

            // Loop through all sightings
            foreach (var sighting in sightings)
            {
                // Add each sighting's Id to the list
                ids.Add(sighting.Id);
            }

            // Convert the list to an array and return it
            int[] idsArray = ids.ToArray();
            Console.WriteLine("SIGHTINGS ARRAY");
            Console.WriteLine(string.Join(", ", idsArray));
            return ids.ToArray();
        }
        
        private void CreateInfoCards()
        {
            var whales = App.WhaleInfoRepo.GetAllWhales(); 

            foreach (var whale in whales)
            {
                var card = new VerticalStackLayout { Spacing = 10 };
                var whaleImage = new Image { Source = ImageSource.FromUri(new Uri(whale.Img)), HeightRequest = 100 };
                var nameLabel = new Label { Text = whale.Name, FontSize = 18, FontAttributes = FontAttributes.Bold };
                var speciesLabel = new Label { Text = $"Species: {whale.Species}", FontSize = 14 };
                var sexLabel = new Label { Text = $"Sex: {whale.Sex}", FontSize = 14 };
                var descriptionLabel = new Label { Text = whale.Bio, FontSize = 14 };

                card.Children.Add(whaleImage);
                card.Children.Add(nameLabel);
                card.Children.Add(speciesLabel);
                card.Children.Add(sexLabel);
                card.Children.Add(descriptionLabel);

                cardsPanel.Children.Add(card);
            }
        }
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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

public class Encounter
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

public class Individual
{
    public int id { get; set; }
    public string species { get; set; }
    public string alternateId { get; set; }
    public object nickname { get; set; }
    public object sex { get; set; }
    public Avatar avatar { get; set; }
}

public class Location
{
    public double lat { get; set; }
    public double lng { get; set; }
}

public class Photo
{
    public int id { get; set; }
    public string type { get; set; }
    public string thumbUrl { get; set; }
    public string url { get; set; }
}

public class Root
{
    public Individual individual { get; set; }
    public object bio { get; set; }
    public List<Encounter> encounters { get; set; }
    public List<Photo> photos { get; set; }
}
