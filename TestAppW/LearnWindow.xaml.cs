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
    /// Interaction logic for LearnWindow.xaml
    /// </summary>
    public partial class LearnWindow : Window
    {
        string dir;
        public LearnWindow()
        {
            InitializeComponent();
        }

        public LearnWindow(string currentDir)
        {
            dir = currentDir;
            InitializeComponent();
            DirectoryInfo d = new DirectoryInfo(System.IO.Path.Combine(dir, "lecture")); // как же
            FileInfo[] Files = d.GetFiles("*.rtf"); // Собираем файлы RTF.
            foreach (FileInfo file in Files)
            {
                var lect = new TextBlock();
                lect.Text = file.Name.Remove(file.Name.Length - 4);
                LectureList.Items.Add(lect);
            }
        }


        private void LectureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextRange textRange;
            FileStream fileStream;
                string lectureName = ((TextBlock)LectureList.Items[LectureList.SelectedIndex]).Text;
                string file = System.IO.Path.Combine(dir, "lecture", lectureName + ".rtf");
                if (System.IO.File.Exists(file))
                {
                    textRange = new TextRange(LectureView.Document.ContentStart, LectureView.Document.ContentEnd);
                    using (fileStream = new System.IO.FileStream(file, System.IO.FileMode.OpenOrCreate))
                    {
                        textRange.Load(fileStream, System.Windows.DataFormats.Rtf);
                    }
                }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            TestWindow test = new TestWindow(dir);
            test.Show();
            Close();
        }
    }
}

