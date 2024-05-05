using WhaleTrail.Data;

namespace WhaleTrail
{
    public partial class App : Application
    {
        public static SightingsRepo SightingsRepo { get; set; }
        public static WhaleInfoRepo WhaleInfoRepo { get; set; }

        public App(SightingsRepo sightingsRepo, WhaleInfoRepo whaleInfoRepo)
        {
            InitializeComponent();

            MainPage = new AppShell();

            SightingsRepo = sightingsRepo;
            
            WhaleInfoRepo = whaleInfoRepo;
        }
    }
}
