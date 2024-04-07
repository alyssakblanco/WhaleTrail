namespace WhaleTrail
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // go to the My Trips - Current first
            Task.Run(async () => await Shell.Current.GoToAsync("//myTripTab/Current"));
        }
    }
}
