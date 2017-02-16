using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public string AppData;
        public StartWindow()
        {
            InitializeComponent();

            // Combine the base folder with your specific folder....
            AppData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestApp");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(AppData))
                Directory.CreateDirectory(AppData);
            if (!Directory.Exists(System.IO.Path.Combine(AppData,"pack")))
                Directory.CreateDirectory(System.IO.Path.Combine(AppData, "pack"));
            if (!Directory.Exists(System.IO.Path.Combine(AppData, "temp")))
                Directory.CreateDirectory(System.IO.Path.Combine(AppData, "temp"));



            UpdateList();
        }

        private void UpdateList()
        {
            DirectoryInfo d = new DirectoryInfo(System.IO.Path.Combine(AppData, "pack"));//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.zip"); //Getting zip files
            TestList.Items.Clear();
            foreach (FileInfo file in Files)
            {
                var lect = new TextBlock() { Text = file.Name.Remove(file.Name.Length - 4) };
                TestList.Items.Add(lect);
            }
        }
        public void Unpack(string file)
        {
            string tempDir = System.IO.Path.Combine(AppData, "temp");
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            Directory.CreateDirectory(tempDir);
            System.IO.Compression.ZipFile.ExtractToDirectory(file, System.IO.Path.Combine(AppData, "temp"), Encoding.GetEncoding(860));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (TestList.SelectedIndex == -1) return;
            string  testName = ((TextBlock)TestList.Items[TestList.SelectedIndex]).Text;
            string path = System.IO.Path.Combine(AppData, "pack", testName + ".zip");
            Unpack(path);
            TestWindow test = new TestWindow(System.IO.Path.Combine(AppData, "temp"));
            test.Show();
            Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (TestList.SelectedIndex == -1) return;
            string testName = ((TextBlock)TestList.Items[TestList.SelectedIndex]).Text;
            string path = System.IO.Path.Combine(AppData, "pack", testName + ".zip");
            Unpack(path);

            LearnWindow test = new LearnWindow(System.IO.Path.Combine(AppData, "temp"));
            test.Show();
            Close();
        }

        private void TestList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        File.Copy(file, System.IO.Path.Combine(AppData, "pack", System.IO.Path.GetFileName(file)), true);
                    }

                }
                UpdateList();
            }
        }
    }
}
