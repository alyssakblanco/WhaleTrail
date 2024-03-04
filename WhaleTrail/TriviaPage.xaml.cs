namespace WhaleTrail
{
    public partial class TriviaPage : ContentPage
    {

        private int currentViewIndex = 0;

        public TriviaPage()
        {
            InitializeComponent();
            contentContainer.Content = GetContentView(0);
        }

        private void OnChangeContentClicked(object? sender, EventArgs e)
        {
            currentViewIndex++;
            contentContainer.Content = GetContentView(currentViewIndex);
        }

        private void OnResetClicked(object? sender, EventArgs e)
        {
            currentViewIndex = 0;
            contentContainer.Content = GetContentView(currentViewIndex);
        }

        private View GetContentView(int index)
        {
            var nextButton = new Button { Text = "Next" };
            nextButton.Clicked += OnChangeContentClicked;

            switch (index)
            {
                case 0:
                    var startButton = new Button { Text = "Start" };
                    startButton.Clicked += OnChangeContentClicked;

                    return new StackLayout
                    {
                        Children =
                        {
                            new Label { Text = "Welcome to the Game" },
                            startButton
                        }
                    };
                case 1:
                    return new StackLayout
                    {
                        Children =
                        {
                            new Label { Text = "Question 1" },
                            nextButton
                        }
                    };
                case 2:
                    return new StackLayout
                    {
                        Children =
                        {
                            new Label { Text = "Question 2" },
                            nextButton
                        }
                    };
                default:
                    var resetButton = new Button { Text = "Reset" };
                    resetButton.Clicked += OnResetClicked;

                    return new StackLayout
                    {
                        Children =
                        {
                            new Label { Text = "End Quiz" },
                            resetButton
                        }
                    };
            }
        }
    }
}
