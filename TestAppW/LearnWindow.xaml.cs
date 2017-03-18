using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using TestApp.Libs;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for LearnWindow.xaml
    /// </summary>
    public partial class LearnWindow : Window
    {
        string currentDirectory;
        public LearnWindow()
        {
            InitializeComponent();
        }
  
        public LearnWindow(string dir)
        {
            currentDirectory = dir;
            InitializeComponent();
            DirectoryInfo d = new DirectoryInfo(Path.Combine(currentDirectory, "lecture")); // как же
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
            string file = Path.Combine(currentDirectory, "lecture", lectureName + ".rtf");
            if (File.Exists(file))
            {
                textRange = new TextRange(LectureView.Document.ContentStart, LectureView.Document.ContentEnd);
                using (fileStream = new FileStream(file, FileMode.OpenOrCreate))
                {
                    textRange.Load(fileStream, DataFormats.Rtf);
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1) Helper.ShowHelp();
        }

        private void BeginTest_Click(object sender, RoutedEventArgs e)
        {
            TestWindow test = new TestWindow(currentDirectory);
            test.Show();
            Close();
        }
    }
}

