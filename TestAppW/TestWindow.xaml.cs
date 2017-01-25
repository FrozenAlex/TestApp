using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TestApp.Methods;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int current;
        private Test test;
        private Question currentQuestion
        {
            get
            {
                return test.Questions[current-1];
            }
            set
            {
                test.Questions[current] = value;
            }
        } // Dat Question
        private DispatcherTimer timer = null;
        private ulong timeLeft;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void timerStart()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            timeLeft--;
            timeLabel.Content = timeLeft;
        }
   
        private void startTest()
        {
            timerStart();
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            test = new Test();
            test.Time = 3;
            test.Author = "Alex Brown";
            test.Encrypted = false;
            test.Name = "Demo Quiz";
            for (int i = 0; i < 100; i++) { 
                TextQuestion question = new TextQuestion();
                question.Answers = new List<Answer>();
                question.Cost = i+1;
                question.Text = "Holy cow " + i;
                Answer answer = new Answer();
                answer.Text = "Totall right" + i;
                answer.Right = true;
                question.Answers.Add(answer);
                Answer wrong = new Answer();
                wrong.Text = "Totally right in quotes" + i;
                wrong.Right = false;
                question.Answers.Add(wrong);

                test.Questions.Add(question);
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Вопросы (*.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                test.Save(dlg.FileName);
                test = null;
            }

            
        }
        private void loadTest()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Вопросы (*.xml)|*.xml";
            dlg.CheckFileExists = true;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == true)
            {
                test = new Test(dlg.FileName);
                current = 1;
                this.Title = test.Name;
                timeLeft = test.Time;
                refresh();
            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            loadTest();
        }

        private void button_Navigation(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Name == "nextButton") current++;
            if (button.Name == "prevButton") current--;
            refresh();
        }

        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            currentQuestion.Answers.Add(new Answer());
        }

        private void refresh()
        {
            // Enable buttons
            prevButton.IsEnabled = (current > 1);
            nextButton.IsEnabled = (current < test.Questions.Count);
            DataContext = currentQuestion;
            questionNum.Content = current;
        }



        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            startTest();
        }

        private void GradeButton_Click(object sender, RoutedEventArgs e)
        {
            timeLabel.Content = test.Grade();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите прервать тестирование?","Тест", MessageBoxButton.YesNo)==MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
