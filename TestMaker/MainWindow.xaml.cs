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
using TestApp.Models;
using System.IO;

namespace TestMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Test test = new Test();
        public int current;
        
        private Question currentQuestion
        {
            get
            {return test.Questions[current];}
            set
            {test.Questions[current] = value;}
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
                //test.Load
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
            //var q = new TextQuestion();
            //q.Text = "Text";
            //q.Cost = 1;
           // var a = new Answer();
           // a.Text = "aaaa";
           // a.Right = false;
            //listBox.DataContext = null;
           // test.Questions.Add(q); //ItemsControl.ItemsSource.

            refresh();
        }


        public void Pack(string folder, string zip)
        {
            if (Directory.Exists(folder))
            {
                System.IO.Compression.ZipFile.CreateFromDirectory(folder, zip, System.IO.Compression.CompressionLevel.Optimal,false,Encoding.UTF8);
            }
        }

        public void Unpack(string zip, string reciever)
        {
            if (Directory.Exists(reciever))
                Directory.Delete(reciever, true);
            Directory.CreateDirectory(reciever);
            System.IO.Compression.ZipFile.ExtractToDirectory(zip, reciever, Encoding.UTF8);
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
        }

        private void UpdateView()
        {
            //QuestionImage.Source = cImage;
            //if (cImage != null) QuestionImage.Visibility = Visibility.Visible;
            //else QuestionImage.Visibility = Visibility.Collapsed;

            //QuestionText.Text = cQuestion.Text; // Текст вопроса
            //QuestionNumber.Text = "Вопрос " + (Current + 1) + " из " + test.Questions.Count;

            //// Кнопки <>
            //prevButton.IsEnabled = (Current >= 1);
            //nextButton.IsEnabled = (Current < test.Questions.Count - 1);
            //// Visibility switch
            //SelectContainer.Visibility = (cQuestion is Question.Select) ? Visibility.Visible : Visibility.Collapsed;
            //RadioContainer.Visibility = (cQuestion is Question.Radio) ? Visibility.Visible : Visibility.Collapsed;
            //TextContainer.Visibility = (cQuestion is Question.Edit) ? Visibility.Visible : Visibility.Collapsed;

            //// Clean all
            //SelectAnswer.Items.Clear();
            //RadioAnswer.Items.Clear();

            //// Get data
            //if (cQuestion is Question.Select)
            //{
            //    foreach (var answer in ((Question.Select)cQuestion).Answers)
            //    {
            //        var checkbox = new CheckBox();
            //        checkbox.Content = answer.Text;
            //        checkbox.IsChecked = answer.Selected;
            //        SelectAnswer.Items.Add(checkbox);
            //    }
            //}

            //if (cQuestion is Question.Radio)
            //{
            //    foreach (var answer in ((Question.Radio)cQuestion).Answers)
            //    {
            //        var TextBlock = new TextBlock();
            //        TextBlock.Text = answer.Text;
            //        RadioAnswer.Items.Add(TextBlock);
            //    }
            //    RadioAnswer.SelectedIndex = ((Question.Radio)cQuestion).Selected;
            //}

            //if (cQuestion is Question.Edit)
            //{
            //    TextAnswer.Text = ((Question.Edit)cQuestion).wrote;
            //}
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Вопросы (*.xml)|*.xml";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;
            if (myDialog.ShowDialog() == true)
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Filter = "Все файлы (*.*)|*.* ";
                //sd.CheckFileExists = true;
                if (sd.ShowDialog() == true)
                {
                    Pack(System.IO.Path.GetDirectoryName(myDialog.FileName),sd.FileName);
                }
            }
        }
    }
}
