using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using TestApp.Methods;
using System.ComponentModel;

namespace TestMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public int current;
        private Test test;
        private Question currentQuestion
        {
            get
            {
                return test.Questions[current];
            }
            set
            {
                test.Questions[current] = value;
                Notify("SelectedIndex");
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Вопросы (*.xml)|*.xml";
            dlg.CheckFileExists = true;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == true)
            {
                listBox.DataContext = null;
                CurrentQuestion.DataContext = null;
                test = new Test(dlg.FileName);
                current = 1;
                refresh();
            }
        }
        private void refresh()
        {
            listBox.DataContext = test;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Вопросы (*.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                test.Save(dlg.FileName);
                test = null;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: заставить работать добавление вопроса и ответа + удаление
            var q = new TextQuestion();
            q.Text = "Text";
            q.Cost = 1;
            var a = new Answer();
            a.Text = "aaaa";
            a.Right = false;
            listBox.DataContext = null;
            test.Questions.Add(q); //ItemsControl.ItemsSource.

            refresh();
        }

        private void QuestionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            current = ((ListBox)sender).SelectedIndex;
            if (current != -1)
            {
                CurrentQuestion.Content = current;
                TextQuestion.DataContext = currentQuestion;
                AnswerList.DataContext = currentQuestion;
            }
            
        }

        private void image_Drop(object sender, DragEventArgs e)
        {
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            
            ((Image) sender).Source = Jp;
        }
    }
}
