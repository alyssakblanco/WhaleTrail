namespace WhaleTrail
{
    public partial class WhalesPage : ContentPage
    {
        private int currentViewIndex = 0;
        private Button? whale1Button;
        private Button? whale2Button;
        private StackLayout? whalesLayout;

        public WhalesPage()
        {
            InitializeComponent();
            contentContainer.Content = GetContentView(0);
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