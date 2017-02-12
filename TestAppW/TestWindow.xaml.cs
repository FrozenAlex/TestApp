using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TestApp.Models;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        string currentDir;
        public Test test = new Test();
        private DispatcherTimer timer = null;
        private ulong timeLeft;

        public TestWindow()
        {
            InitializeComponent();
        }

        public TestWindow(string folderPath)
        {
            currentDir = folderPath;
            test.Load(Path.Combine(folderPath,"test.xml"));
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
        }

        public int Current;
        public Question cQuestion
        {
            get
            {
                if (test.Questions != null)
                    return test.Questions[Current];
                return null;
            }
            set
            {
                test.Questions[Current] = value;
            }
        }

        private void startTest()
        {
            timerStart();
        }
        private void loadTest()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Вопросы (*.xml)|*.xml";
            dlg.CheckFileExists = true;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == true)
            {
                test.Load(dlg.FileName);
                UpdateView();
            }
        }
        
        public BitmapImage cImage
        {
            get
            {
                if (Current >= test.Questions.Count || Current < 0) return null;
                if (test.Questions[Current].Image == null)
                {
                    return null;
                }
                else
                {
                    var uri = new Uri(Path.Combine(currentDir, "image",test.Questions[Current].Image));
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = uri;
                    image.EndInit();
                    var bit = image;
                    return bit;
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            TestContainer.Visibility = Visibility.Collapsed;
            ResultContainer.Visibility = Visibility.Visible;
            BeginButton.IsEnabled = true;
            EndButton.IsEnabled = false;
            float result = test.Grade();
            if (result < 0.20)
            {
                ResultText.Text = "Ужасно";   
            }
            else if (result < 0.40)
            {
                ResultText.Text = "Плохо";
            }
            else if (result < 0.55)
            {
                ResultText.Text = "Так себе...";
            }
            else if (result < 0.79)
            {
                ResultText.Text = "Хорошо";
            }
            else if (result < 1)
            {
                ResultText.Text = "Почти отлично";
            }
            else if (result == 1)
            {
                ResultText.Text = "Отлично";
            }
            ResultWrongNum.Text = "Правильных ответов: " + (result * 100).ToString() + "%";
        }

        private void button_Click_2(object sender, RoutedEventArgs e)
        {
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            startTest();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           if (MessageBox.Show("Вы действительно хотите прервать тестирование?","Тест", MessageBoxButton.YesNo)==MessageBoxResult.No)
            {
             e.Cancel = true;
           }
        }

        private void GetData()
        {
            // Get user answers
            if (cQuestion is Question.Select)
            {
                int i = 0;
                foreach (var answer in SelectAnswer.Items)
                {
                    ((Question.Select)cQuestion).Answers[i].Selected = (bool)((CheckBox)answer).IsChecked;
                    i++;
                }
            }

            if (cQuestion is Question.Radio)
            {
                ((Question.Radio)cQuestion).Selected = RadioAnswer.SelectedIndex;
            }

            if (cQuestion is Question.Edit)
            {
                ((Question.Edit)cQuestion).wrote = TextAnswer.Text;
            }
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            GetData(); // Get user input
            Button button = sender as Button;
            if (button.Name == "nextButton") Current++;
            if (button.Name == "prevButton") Current--;
            UpdateView(); 
        }
        private void UpdateView()
        {
            QuestionImage.Source = cImage;
            if (cImage != null) QuestionImage.Visibility = Visibility.Visible;
            else QuestionImage.Visibility = Visibility.Collapsed;

            QuestionText.Text = cQuestion.Text; // Текст вопроса
            QuestionNumber.Text = "Вопрос " + (Current+1) + " из " + test.Questions.Count;

            // Кнопки <>
            prevButton.IsEnabled = (Current >= 1);
            nextButton.IsEnabled = (Current < test.Questions.Count - 1);
            // Visibility switch
            SelectContainer.Visibility = (cQuestion is Question.Select) ? Visibility.Visible : Visibility.Collapsed;
            RadioContainer.Visibility = (cQuestion is Question.Radio) ? Visibility.Visible : Visibility.Collapsed;
            TextContainer.Visibility = (cQuestion is Question.Edit) ? Visibility.Visible : Visibility.Collapsed;

            // Clean all
            SelectAnswer.Items.Clear();
            RadioAnswer.Items.Clear();

            // Get data
            if (cQuestion is Question.Select)
            {
                foreach (var answer in ((Question.Select)cQuestion).Answers)
                {
                    var checkbox = new CheckBox();
                    checkbox.Content = answer.Text;
                    checkbox.IsChecked = answer.Selected;
                    SelectAnswer.Items.Add(checkbox);
                }
            }

            if (cQuestion is Question.Radio)
            {
                foreach (var answer in ((Question.Radio)cQuestion).Answers)
                {
                    var TextBlock = new TextBlock();
                    TextBlock.Text = answer.Text;
                    RadioAnswer.Items.Add(TextBlock);
                }
                RadioAnswer.SelectedIndex = ((Question.Radio)cQuestion).Selected;
            }

            if (cQuestion is Question.Edit)
            {
                TextAnswer.Text = ((Question.Edit)cQuestion).wrote;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void OnDragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void OnMinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void OnMaximizeWindow(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
        }
        private void OnCloseWindow(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void BeginButton_Click(object sender, RoutedEventArgs e)
        {
            ResultContainer.Visibility = Visibility.Collapsed;
            TestContainer.Visibility = Visibility.Visible;
            BeginButton.IsEnabled = false;
            EndButton.IsEnabled = true;
            UpdateView();
        }
    }
}
