﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TestApp.Libs;
using TestApp.Models;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        // Переменные
        string currentDir; // Папка с тестом
        public Test test = new Test(); // Тест
        private DispatcherTimer timer = null; // Таймер
        private ulong TimeLeft; // Время до остановки теста
        private bool isTesting = false; // Флаг запущен ли тест
        public int Current; // Текущий вопрос

        // Свойства
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
        public BitmapImage cImage
        {
            get
            {
                if (Current >= test.Questions.Count || Current < 0) return null;
                if (test.Questions[Current].Image == null) return null;
                else
                {
                    var uri = new Uri(Path.Combine(currentDir, "image",test.Questions[Current].Image));
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = uri;
                    image.EndInit();
                    return image;
                }
            }
        }

        // Конструктор
        public TestWindow()
        {
            InitializeComponent();
        }
        public TestWindow(string folderPath)
        {
            currentDir = folderPath;
            test.Load(Path.Combine(folderPath,"test.xml"));
            InitializeComponent();

            Title = test.Name; // Установить заголовок окна на название теста
            
            // Заполнить приветствие
            TestName.Text = test.Name;
            if (String.IsNullOrWhiteSpace(test.Author)) TestAuthor.Visibility = Visibility.Collapsed; // Скрыть автора если его нет
            else TestAuthor.Text = "Автор теста: " + test.Author; // Вывести автора
            TestQuestionsNum.Text = "Вопросов: " + test.Questions.Count; // Вывести количество вопросов
            if (test.Time<1) TestTime.Text = "Времени на тест: не ограничено"; // Время на тест
            else TestTime.Text = "Времени на тест: " + TimeSpan.FromSeconds(test.Time).ToString(@"mm\:ss");
            WelcomeContainer.Visibility = Visibility.Visible; // Видимость контейнера приветствия

            
            if (test.Time > 1) // Показать таймер если время ограничено иначе скрыть
            {
                TimeContainer.Visibility = Visibility.Visible;
                TimeLeft = test.Time;
                TimeLeftText.Text = TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss"); // Форматированый вывод времени
            }
            else TimeContainer.Visibility = Visibility.Collapsed;
        }

        // Старт таймера
        private void TimerStart()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();
        }
        // Функция, вызываемая по срабатыванию таймера
        private void TimerTick(object sender, EventArgs e)
        {
            TimeLeft--; 
            TimeLeftText.Text = TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss");
            if (TimeLeft <= 0) // Если времени не осталось то закончить тест за пользователя
            {
                (sender as DispatcherTimer).Stop();
                EndTest(); 
            }
        }
        
        // Функция начать тест
        private void StartTest()
        {
            // Установить флаг теста (Запущен ли тест)
            isTesting = true;

            // Инициализация теста
            test.Clean(); // Сбросить все ответы
            test.Shuffle(); // Перемешать тест
            if (test.Questions != null) Current = 0; // Сбросить текущий вопрос на 1й
            
            // Кнопки начать тест и закончить тест
            BeginButton.IsEnabled = false;
            EndButton.IsEnabled = true;

            // Если время больше еденицы то начинаем считать время
            if (test.Time > 1)
            {
                TimeLeft = test.Time;
                TimerStart();
            }

            UpdateView(); // Заполнить вид

            // Видимость теста и результата
            ResultContainer.Visibility = Visibility.Collapsed;
            TestContainer.Visibility = Visibility.Visible;
            WelcomeContainer.Visibility = Visibility.Collapsed;
        }
        // Функция закончить тест и вывести результат
        private void EndTest()
        {
            GetData(); // Получить ответ на текущий вопрос

            // Видимость контейнеров
            TestContainer.Visibility = Visibility.Collapsed;
            ResultContainer.Visibility = Visibility.Visible;
            WelcomeContainer.Visibility = Visibility.Collapsed;

            // Отключение и включение нужных кнопок
            BeginButton.IsEnabled = true;
            EndButton.IsEnabled = false;

            Result result = test.Grade(); // Получить оценку
            // Показать оценку в текстовом варианте
            if (result.Score < 0.20) ResultText.Text = "Ужасно";
            else if (result.Score < 0.40) ResultText.Text = "Плохо";
            else if (result.Score < 0.55) ResultText.Text = "Так себе...";
            else if (result.Score < 0.79) ResultText.Text = "Хорошо";
            else if (result.Score < 1) ResultText.Text = "Почти отлично";
            else if (result.Score == 1) ResultText.Text = "Отлично";
            
            // Statistics
            ResultPercent.Text = String.Format("Правильно {0:0.00}%", (result.Score * 100));
            ResultRightNum.Text = "Верных ответов: " + result.Right;
            ResultWrongNum.Text = "Неверных ответов: " + result.Wrong;
            ResultUnansweredNum.Text = "Не отвеченых: " + result.NotAnswered;

            if ((result.Wrong) == 0) ResultWrong.Visibility = Visibility.Collapsed;

            List<Question> wrong = test.Wrong; // Получение неверных ответов
            // Вывод неправильных ответов
            ResultWrongQuestions.Text = "";
            foreach (Question q in wrong)
            {
                ResultWrongQuestions.Text += q.Text + "\n";
            }
            if (timer != null) timer.Stop(); // Stop timer if exists

            isTesting = false; // Reset test flag
        }
        
        // Функция получения ответа пользователя на текущий вопрос
        private void GetData()
        {
            // Получить ответы пользователя
            if (cQuestion is Question.Select)
            {
                foreach (var answer in SelectAnswer.Items)
                {
                    ((Question.Select)cQuestion).Answers[SelectAnswer.Items.IndexOf(answer)].Selected = (bool)(answer as CheckBox).IsChecked;
                }
            }

            if (cQuestion is Question.Radio)
                ((Question.Radio)cQuestion).Selected = RadioAnswer.SelectedIndex;
            
            if (cQuestion is Question.Edit)
                ((Question.Edit)cQuestion).wrote = TextAnswer.Text;
        }
        // Функция, заполняющая вопросы и ответы, и управляющая видом окна в режиме тестирования
        private void UpdateView()
        {
            // Изображение
            QuestionImage.Source = cImage;
            QuestionImage.Visibility = (cImage != null) ? Visibility.Visible : Visibility.Collapsed; // Видимость контейнера изображения

            // Заполнение номера вопроса и текста вопроса
            QuestionText.Text = cQuestion.Text; // Текст вопроса
            QuestionNumber.Text = "Вопрос " + (Current + 1) + " из " + test.Questions.Count; // Номер вопроса

            // Включение и отключение кнопок навигации
            prevButton.IsEnabled = (Current >= 1);
            nextButton.IsEnabled = (Current < test.Questions.Count - 1);

            // Видимость контейнеров для различных типов вопросов
            SelectContainer.Visibility = (cQuestion is Question.Select) ? Visibility.Visible : Visibility.Collapsed;
            RadioContainer.Visibility = (cQuestion is Question.Radio) ? Visibility.Visible : Visibility.Collapsed;
            TextContainer.Visibility = (cQuestion is Question.Edit) ? Visibility.Visible : Visibility.Collapsed;

            // Очистить все контейнеры
            SelectAnswer.Items.Clear();
            RadioAnswer.Items.Clear();

            // Изменить текст подсказки
            if (cQuestion is Question.Edit) AnswerHelper.Content = "Введите ответ на вопрос:";
            if (cQuestion is Question.Radio) AnswerHelper.Content = "Выберите один правильный ответ:";
            if (cQuestion is Question.Select) AnswerHelper.Content = "Выберите один или несколько правильных ответов:";

            // Заполнение различных контейнеров с ответами в зависимости от типа вопроса
            if (cQuestion is Question.Select)
            {
                foreach (var answer in (cQuestion as Question.Select).Answers)
                {
                    var checkbox = new CheckBox() { Content = answer.Text, IsChecked = answer.Selected };
                    SelectAnswer.Items.Add(checkbox);
                }
            }

            // Если вопрос является вопросом выбора 1го правильного ответа
            if (cQuestion is Question.Radio)
            {
                foreach (var answer in (cQuestion as Question.Radio).Answers)
                {
                    var TextBlock = new TextBlock() { Text = answer.Text };
                    RadioAnswer.Items.Add(TextBlock);
                }
                RadioAnswer.SelectedIndex = (cQuestion as Question.Radio).Selected;
            }
            if (cQuestion is Question.Edit) TextAnswer.Text = (cQuestion as Question.Edit).wrote;
        }

        // Обработчик кнопок навигации
        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            GetData(); // Получить ответ на текущий вопрос
            Button button = sender as Button;
            if (button.Name == "nextButton") Current++;
            if (button.Name == "prevButton") Current--;
            UpdateView(); 
        }

        // Обработчики нажатий некоторых кнопок (Которые вызывают функцию выше)
        private void BeginButton_Click(object sender, RoutedEventArgs e)
        {
            StartTest();
        }
        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            EndTest();
        }

        // Обработчик закрытия окна для вывода сообщения с подтверждением
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isTesting)
            {
                if (MessageBox.Show("Вы действительно хотите прервать тестирование?", "Тест", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }

        }
        // Обработчики нажатий кнопок
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1) Helper.ShowHelp();
            if (isTesting)
            {
                if (e.Key == Key.Left)
                {
                    GetData(); // Получить ответ на текущий вопрос
                    if (Current < test.Questions.Count) Current--;
                    UpdateView();
                }
                if (e.Key == Key.Right)
                {
                    GetData();
                    if (Current > 0) Current++;
                    UpdateView();
                }
            }
            
        }
    }
}
