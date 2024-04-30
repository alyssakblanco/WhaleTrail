using System.Text.Json;
using WhaleTrail.Models;
using WhaleTrail.Services;

namespace WhaleTrail
{
    public partial class WhalesPage : ContentPage
    {
        private int currentViewIndex = 0;
        private Button? whale1Button;
        private Button? whale2Button;
        private StackLayout? whalesLayout;
        private DataService _dataService;

        public WhalesPage()
        {
            InitializeComponent();
            contentContainer.Content = GetContentView(0);

            _dataService = new DataService();
            LoadDataAsync();

        }

        private async void LoadDataAsync()
        {
            Console.WriteLine("ATTEMPT 1");

            try
            {
                var data = await _dataService.FetchWhaleDataAsync("93975");
                var rootObject = JsonSerializer.Deserialize<Root>(data);
                Console.WriteLine(data);
                Console.WriteLine(rootObject);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void SetupWhalesLayout()
        {
            whale1Button = new Button { Text = "Learn More" };
            whale1Button.Clicked += (sender, e) => OnChangeContentClicked(1);
            whale2Button = new Button { Text = "Learn More" };
            whale2Button.Clicked += (sender, e) => OnChangeContentClicked(2);

            whalesLayout = new StackLayout
            {
                Children =
                {
                    new Label { Text = "Whale Name 1" },
                    whale1Button,
                    new Label { Text = "Whale Name 2" },
                    whale2Button
                }
            };
        }

        private void OnChangeContentClicked(int index)
        {
            currentViewIndex = index;
            contentContainer.Content = GetContentView(currentViewIndex);
        }

        private View GetContentView(int index)
        {
            // The reset button
            var reset = new Button { Text = "Go Back" };
            reset.Clicked += (sender, e) => OnChangeContentClicked(0);

            // Switch based on the index
            switch (index)
            {
                case 1:
                    return new StackLayout
                    {
                        Children =
                        {
                            new Label { Text = "Whale 1" },
                            reset
                        }
                    };
                case 2:
                    return new StackLayout
                    {
                        Children =
                        {
                            new Label { Text = "Whale 2" },
                            reset
                        }
                    };
                default:
                    SetupWhalesLayout(); 
                    return whalesLayout;
            }
        }
    }
}

// JSON CLASSES
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Avatar
{
    public int id { get; set; }
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

