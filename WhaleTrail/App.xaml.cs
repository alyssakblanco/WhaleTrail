using WhaleTrail.Data;

namespace WhaleTrail
{
    public partial class App : Application
    {
        public static SightingsRepo SightingsRepo { get; set; }

        public App(SightingsRepo sightingsRepo)
        {
            InitializeComponent();

            MainPage = new AppShell();

            SightingsRepo = sightingsRepo;
        }
    }
}
