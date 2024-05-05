namespace WhaleTrail
{
    public partial class TriviaPage : ContentPage
    {
        private int quizScore;

        public TriviaPage()
        {
            InitializeComponent();
        }

        public void StartQuiz(object sender, EventArgs e)
        {
            quizScore = 0;
            StartLayout.IsVisible = false;
            QuizLayout.IsVisible = true;
        }

        public void Finish(object sender, EventArgs e)
        {
            if (answer0.IsChecked == true)
            {
                quizScore++;
            }
            if (answer1.IsChecked == true)
            {
                quizScore++;
            }
            if (answer2.IsChecked == true)
            {
                quizScore++;
            }
            if (answer3.IsChecked == true)
            {
                quizScore++;
            }
            if (answer4.IsChecked == true)
            {
                quizScore++;
            }

            score.Text = $"{quizScore} / 5";
            QuizLayout.IsVisible = false;
            EndLayout.IsVisible = true;
        }

        public void Reset(object sender, EventArgs e)
        {
            EndLayout.IsVisible = false;
            StartLayout.IsVisible = true;
        }
    }
}
