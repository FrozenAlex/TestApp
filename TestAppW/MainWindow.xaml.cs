using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TestModel;

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
                return test.questions[current-1]; // current is a little bigger
            }
            set
            {
                test.questions[current] = value;
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
            test.time = 3;
            test.author = "Alex Brown";
            test.encrypted = false;
            test.name = "Demo Quiz";
            for (int i = 0; i < 4; i++) { 
                Question question = new Question("Question number "+i);
                question.answers.Add(new Answer("Right", true));
                question.answers.Add(new Answer("Right2", true));
                question.answers.Add(new Answer("Wrong", false));
                question.answers.Add(new Answer("Wrong2", false));
                question.answers.Add(new Answer("Wrong3", false));
                test.questions.Add(question);
            }
            
            test.SaveToFile("D:\\xml.xml");
            test = null;
        }
        private void loadTest()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Вопросы (*.xml)|*.xml";
            dlg.CheckFileExists = true;
            dlg.Multiselect = false;
            //if (dlg.ShowDialog() == true)
            //{
            //test = new Test(dlg.FileName);"D:\\xml.xml"
                test = new Test("D:\\xml.xml");
                current = 1;
                this.Title = test.name;
                timeLeft = test.time;
                refresh();
            //}
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
            currentQuestion.answers.Add(new Answer());
        }

        private void refresh()
        {
            DataContext = currentQuestion;
           // listBox.ItemsSource = currentQuestion.answers;
            //QuestionText.Text = currentQuestion.text;
            //questionNum.Content = current;
            // Enable buttons
            prevButton.IsEnabled = (current > 1);
            nextButton.IsEnabled = (current < test.questions.Count);
    }



        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            startTest();
        }

        private void GradeButton_Click(object sender, RoutedEventArgs e)
        {
            timeLabel.Content = test.Grade();

        }
    }
}
