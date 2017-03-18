using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TestApp.Libs;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public string AppData;
        string tempDir;
        public StartWindow()
        {
            InitializeComponent();

            // Combine the base folder with your specific folder....
            AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestApp");
            tempDir = Path.Combine(AppData, "temp");

            // Check if app folders exist and if not, create them
            if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData); // AppData/TestApp
            if (!Directory.Exists(Path.Combine(AppData,"pack"))) // AppData/TestApp/pack
                Directory.CreateDirectory(Path.Combine(AppData, "pack"));
            if (!Directory.Exists(Path.Combine(AppData, "temp")))// AppData/TestApp/temp
                Directory.CreateDirectory(Path.Combine(AppData, "temp"));
            UpdateList();
        }

        // Обновить список тестов
        private void UpdateList()
        {
            DirectoryInfo d = new DirectoryInfo(Path.Combine(AppData, "pack"));//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.zip"); //Getting zip files
            TestList.Items.Clear();
            foreach (FileInfo file in Files)
            {
                var lect = new TextBlock() { Text = file.Name.Remove(file.Name.Length - 4) };
                TestList.Items.Add(lect);
            }
        }
        // Распаковка файлов теста во временную папку
        private void UnpackFiles()
        {
            if (TestList.SelectedIndex == -1) return;
            string testName = ((TextBlock)TestList.Items[TestList.SelectedIndex]).Text;
            string path = Path.Combine(AppData, "pack", testName + ".zip");
            Compression.Unpack(path, tempDir);
        }

        // Добавление тестов при перетаскивании
        private void TestList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    if (File.Exists(file))
                        File.Copy(file, Path.Combine(AppData, "pack", Path.GetFileName(file)), true);
                }
                UpdateList();
            }
        }

        // Справка
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1) Helper.ShowHelp();
        }

        // Button listeners
        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            UnpackFiles();
            TestWindow test = new TestWindow(tempDir);
            test.Show();
            Close();
        }

        private void StartLearn_Click(object sender, RoutedEventArgs e)
        {
            UnpackFiles();
            LearnWindow test = new LearnWindow(tempDir);
            test.Show();
            Close();
        }

        private void ShowHelp_Click(object sender, RoutedEventArgs e)
        {
            Helper.ShowHelp();
        }
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (TestList.SelectedIndex == -1) return;
            string testName = ((TextBlock)TestList.Items[TestList.SelectedIndex]).Text;
            string path = Path.Combine(AppData, "pack", testName + ".zip");
            File.Delete(path);
            UpdateList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Вопросы (*.zip)|*.zip";
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == true)
            {
                if (File.Exists(dialog.FileName))
                    File.Copy(dialog.FileName, Path.Combine(AppData, "pack", Path.GetFileName(dialog.FileName)), true);
                UpdateList();
            }
        }
    }
}
