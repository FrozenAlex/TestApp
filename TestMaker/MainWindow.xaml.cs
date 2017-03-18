using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using TestApp.Methods;
using System.ComponentModel;
using TestApp.Models;
using System.IO;
using TestApp;
using TestApp.Libs;

namespace TestMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Test test = new Test();
        public int Current;
        public bool ro = false;

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

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(test.Path)) {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Вопросы (*.xml)|*.xml";
                if (dlg.ShowDialog() == true)
                {
                    test.Name = Field_TestName.Text;
                    test.Author = Field_TestAuthor.Text;
                    test.Time = ulong.Parse(Field_TestTime.Text);
                    test.Save(dlg.FileName);
                }
            }
            else
            {
                test.Name = Field_TestName.Text;
                test.Author = Field_TestAuthor.Text;
                test.Time = ulong.Parse(Field_TestTime.Text);
                test.Save(test.Path);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //Галочный Радио Текстовый
            if (QuestionTypeBox.SelectedIndex < 0 || QuestionTypeBox.SelectedIndex > 2) { return; }
            if (QuestionTypeBox.SelectedIndex == 0)
            {
                var q = new Question.Select() { Text = "Select " + test.Questions.Count, Answers = new List<Question.Answer>() };
                test.Questions.Add(q);
            }
            if (QuestionTypeBox.SelectedIndex == 2)
            {
                var q = new Question.Edit() { Text = "Edit " + test.Questions.Count };
                test.Questions.Add(q);
            }
            if (QuestionTypeBox.SelectedIndex == 1)
            {
                var q = new Question.Radio() { Text = "Radio " + test.Questions.Count, Answers = new List<Question.Answer>() };
                test.Questions.Add(q);
            }
            UpdateList();
        }

        private void QuestionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ro && Current != -1) GetData(); // получить вопрос
            Current = ((ListBox)sender).SelectedIndex; // Получить выбранный вопрос
            if (Current != -1) UpdateView();
        }

        private void image_Drop(object sender, DragEventArgs e)
        {
            //string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
        }
        private void GetData()
        {
            cQuestion.Text = QuestionText.Text;
            // Получить ответы пользователя
            if (cQuestion is Question.Select)
            {
                ((Question.Select)cQuestion).Answers.Clear();
                foreach (var answer in SelectAnswer.Items)
                {
                    CheckBox check = answer as CheckBox;
                    TextBox textBox = check.Content as TextBox;
                    Question.Answer answ = new Question.Answer() { Right = (bool)check.IsChecked, Text = textBox.Text };
                    ((Question.Select)cQuestion).Answers.Add(answ);
                }
            }

            if (cQuestion is Question.Radio)
            {
                ((Question.Radio)cQuestion).Answers.Clear();
                foreach (var answer in RadioAnswer.Items)
                {
                    RadioButton check = answer as RadioButton;
                    TextBox textBox = check.Content as TextBox;
                    Question.Answer answ = new Question.Answer() { Right = (bool)check.IsChecked, Text = textBox.Text };
                    ((Question.Radio)cQuestion).Answers.Add(answ);
                }
            }

            if (cQuestion is Question.Edit)
            {
                ((Question.Edit)cQuestion).answer = TextAnswer.Text;
            }
        }
        private void UpdateView()
        {
            //QuestionImage.Source = cImage;
            //if (cImage != null) QuestionImage.Visibility = Visibility.Visible;
            //else QuestionImage.Visibility = Visibility.Collapsed;

            QuestionText.Text = cQuestion.Text; // Текст вопроса
            QuestionNumber.Text = "Вопрос " + (Current + 1) + " из " + test.Questions.Count; // Номер вопроса

            // Видимость контейнеров для различных типов вопросов
            SelectContainer.Visibility = (cQuestion is Question.Select) ? Visibility.Visible : Visibility.Collapsed;
            RadioContainer.Visibility = (cQuestion is Question.Radio) ? Visibility.Visible : Visibility.Collapsed;
            TextContainer.Visibility = (cQuestion is Question.Edit) ? Visibility.Visible : Visibility.Collapsed;

            // Очистить все контейнеры
            SelectAnswer.Items.Clear();
            RadioAnswer.Items.Clear();

            // Заполнение различных контейнеров с ответами в зависимости от типа вопроса
            if (cQuestion is Question.Select)
            {
                foreach (var answer in (cQuestion as Question.Select).Answers)
                {
                    var edit = new TextBox() { Text = answer.Text };
                    var checkbox = new CheckBox() { Content = edit, IsChecked = answer.Right };
                    SelectAnswer.Items.Add(checkbox);
                }
            }

            // Если вопрос является вопросом выбора 1го правильного ответа
            if (cQuestion is Question.Radio)
            {
                foreach (var answer in (cQuestion as Question.Radio).Answers)
                {
                    var TextBox = new TextBox() { Text = answer.Text };
                    var radio = new RadioButton() { GroupName="group", Content = TextBox, IsChecked = answer.Right };
                    RadioAnswer.Items.Add(radio);
                }
                //RadioAnswer.SelectedIndex = (cQuestion as Question.Radio).Selected;
            }
            if (cQuestion is Question.Edit) TextAnswer.Text = (cQuestion as Question.Edit).answer;
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
                    Compression.Pack(Path.GetDirectoryName(myDialog.FileName), sd.FileName);
                }
            }
        }

        private void UpdateList()
        {
            QuestionList.Items.Clear();
            foreach (Question q in test.Questions)
            {
                var TextBlock = new TextBlock() { Text = q.Text };
                QuestionList.Items.Add(TextBlock);
            }
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Вопросы (*.xml)|*.xml";
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == true)
            {
                test.Load(dialog.FileName);
                test.Path = dialog.FileName;
                Current = 0;
                Field_TestName.Text = test.Name;
                Field_TestAuthor.Text = test.Author;
                Field_TestTime.Text = test.Time.ToString();
                UpdateView();
                UpdateList();
            }
        }

        private void AddAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (cQuestion is Question.Select)
            {
                var edit = new TextBox() { Text = "Ответ " + SelectAnswer.Items.Count };
                var checkbox = new CheckBox() { Content = edit, IsChecked = false };
                SelectAnswer.Items.Add(checkbox);
            }

            // Если вопрос является вопросом выбора 1го правильного ответа
            if (cQuestion is Question.Radio)
            {
                var TextBox = new TextBox() { Text = "Ответ "+ RadioAnswer.Items.Count };
                var radio = new RadioButton() { GroupName = "group", Content = TextBox, IsChecked = false };
                RadioAnswer.Items.Add(radio);
               
            }
        }

        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            GetData();
            test.Name = Field_TestName.Text;
            test.Author = Field_TestAuthor.Text;
            test.Time = ulong.Parse(Field_TestTime.Text);
            TestWindow testWindow = new TestWindow(test);
            testWindow.ShowDialog();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Вопросы (*.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                test.Save(dlg.FileName);
            }
        }

        private void DeleteAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (cQuestion is Question.Select)
            {
                if (SelectAnswer.SelectedIndex >= 0) SelectAnswer.Items.RemoveAt(SelectAnswer.SelectedIndex);
            }

            // Если вопрос является вопросом выбора 1го правильного ответа
            if (cQuestion is Question.Radio)
            {
                if (RadioAnswer.SelectedIndex >= 0) RadioAnswer.Items.RemoveAt(RadioAnswer.SelectedIndex);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionList.SelectedIndex>=0) test.Questions.RemoveAt(QuestionList.SelectedIndex);
            ro = true;
            UpdateList();
            ro = false;
        }

        private void NewTest_Click(object sender, RoutedEventArgs e)
        {
            test.Questions = new List<Question>();
            test.Questions.Add(new Question.Select() { Text = "Select", Answers = new List<Question.Answer>() });
            Current = 0;
            UpdateView();
            UpdateList();
        }
    }
}
