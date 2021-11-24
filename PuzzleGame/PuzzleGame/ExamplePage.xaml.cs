using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PuzzleInterpretation;
using System.Windows.Threading;
using DataAnalysis;

namespace PuzzleGame
{
    /// <summary>
    /// Логика взаимодействия для ExamplePage.xaml
    /// </summary>
    public partial class ExamplePage : ContentControl
    {
        public TestingParameters TestingParams { get; set; }

        private List<MatchesPuzzle> puzzles;

        private int curPuzzle = 0;

        private DispatcherTimer timer = new DispatcherTimer();

        public ExamplePage(TestingParameters parameters)
        {
            TestingParams = parameters;
            InitializeComponent();
            puzzles = TestingParams.Puzzles;
            numAttemptsLeft.Text = TestingParams.NumAttempts.ToString();
            timeLeft.Text = TestingParams.AttemptDuration.ToString();
            movesLeft.Text = puzzles[curPuzzle].MatchesToMoveLeft.ToString();

            foreach (var puzzle in puzzles)
            {
                puzzle.AttemptsLeft = parameters.NumAttempts;
                puzzle.TimeLeft = parameters.AttemptDuration;
            }

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(GameTick);

            puzzles[curPuzzle].RenderSlots(canvas, 20, 40, TestingParams.AreSlotsVisible);
            puzzles[curPuzzle].Render(canvas, 20, 40);

            timer.Start();
            DataWriter.DataConsumer("Probe_start", System.DateTime.Now, 0, 0, "" + curPuzzle + " Attemp " + (TestingParams.NumAttempts - puzzles[curPuzzle].AttemptsLeft + 1) + " start");
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (--puzzles[curPuzzle].TimeLeft > 0)
            {
                timeLeft.Text = puzzles[curPuzzle].TimeLeft.ToString();
                movesLeft.Text = puzzles[curPuzzle].MatchesToMoveLeft.ToString();
            }
            else
            {
                timeLeft.Text = "0";
                timer.Stop();
                MessageBox.Show("Время на решения головоломки истекло", "Увы...", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                DataWriter.DataConsumer("Probe_end", System.DateTime.Now, 0, 0, "" + curPuzzle + " Attemp " + (TestingParams.NumAttempts - puzzles[curPuzzle].AttemptsLeft + 1) + " end");
                NextPuzzle();
            }
        }

        private void NewAttempt(object sender, RoutedEventArgs e)
        {
            if (--puzzles[curPuzzle].AttemptsLeft > 0)
            {
                DataWriter.DataConsumer("Probe_end", System.DateTime.Now, 0, 0, "" + curPuzzle + " Attemp " + (TestingParams.NumAttempts - puzzles[curPuzzle].AttemptsLeft) + " end");
                DataWriter.DataConsumer("Probe_start", System.DateTime.Now, 0, 0, "" + curPuzzle + " Attemp " + (TestingParams.NumAttempts - puzzles[curPuzzle].AttemptsLeft + 1) + " start");
                puzzles[curPuzzle].MatchesToMoveLeft = puzzles[curPuzzle].MatchesToMove;
                numAttemptsLeft.Text = puzzles[curPuzzle].AttemptsLeft.ToString();
                movesLeft.Text = puzzles[curPuzzle].MatchesToMove.ToString();
                canvas.Children.Clear();
                puzzles[curPuzzle].RenderSlots(canvas, 20, 40, TestingParams.AreSlotsVisible);
                puzzles[curPuzzle].Render(canvas, 20, 40);
                if(puzzles[curPuzzle].AttemptsLeft == 1)
                {
                    newAttempt.IsEnabled = false;
                }
            }
            else
            {
                numAttemptsLeft.Text = "0";
            }
        }

        private void NextPuzzleClick(object sender, RoutedEventArgs e)
        {
            NextPuzzle();
        }

        private void NextPuzzle()
        {
            DataWriter.DataConsumer("Probe_end", System.DateTime.Now, 0, 0, "" + curPuzzle + " Attemp " + (TestingParams.NumAttempts - puzzles[curPuzzle].AttemptsLeft + 1) + " end");
            timer.Stop();
            if (TestingParams.IsFeedbackNeeded && timeLeft.Text != "0")
            {
                if (puzzles[curPuzzle].IsSolved)
                    MessageBox.Show("Задание решено верно", "Поздравляем!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                else
                    MessageBox.Show("Задание решено неверно", "Увы...", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }

            if (++curPuzzle < puzzles.Count)
            {
                numAttemptsLeft.Text = puzzles[curPuzzle].AttemptsLeft.ToString();
                newAttempt.IsEnabled = true;
                canvas.Children.Clear();
                puzzles[curPuzzle].RenderSlots(canvas, 20, 40, TestingParams.AreSlotsVisible);
                puzzles[curPuzzle].Render(canvas, 20, 40); 
                timer.Start();
                DataWriter.DataConsumer("Probe_start", System.DateTime.Now, 0, 0, "" + curPuzzle + " Attemp " + (TestingParams.NumAttempts - puzzles[curPuzzle].AttemptsLeft + 1) + " start");
            }
            else
            {
                Content = new FinalWindow();
            }
        }
    }
}
