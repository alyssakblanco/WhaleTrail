namespace WhaleTrail
{
    public partial class TriviaPage : ContentPage
    {
        public int finalScore = 0;

        public TriviaPage()
        {
            InitializeComponent();
            quizQuestions.IsVisible = false;
        }

        void StartQuizClicked(System.Object sender, System.EventArgs e)
        {
            quizQuestions.IsVisible = true;
            startQuizBtn.IsVisible = false;
        }

        void FinishQuizClicked(System.Object sender, System.EventArgs e)
        {
            quizQuestions.IsVisible = false;
            ScoreQuiz();
            score.IsVisible = true;
        }

        void ScoreQuiz()
        {
            if (q1.IsChecked) { finalScore += 1; }
            if (q2.IsChecked) { finalScore += 1; }
            if (q3.IsChecked) { finalScore += 1; }
            if (q4.IsChecked) { finalScore += 1; }
            if (q5.IsChecked) { finalScore += 1; }
        }
    }
}
