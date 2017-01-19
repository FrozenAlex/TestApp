using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using TestApp.Methods;

namespace TestApp.Methods
{

    [XmlRoot("Test")]
    [XmlInclude(typeof(Test))]
    public partial class Test  // One set of questions
    {
        [XmlIgnore]
        private string Path;
        [XmlAttribute]
        public ulong Id { get; set; }
        [XmlAttribute]
        public ulong Time { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Author { get; set; }
        [XmlAttribute]
        public bool Encrypted { get; set; }

        [XmlArray("questions")]
        [XmlArrayItem("question")]
        public List<Question> Questions;
    }



    public partial class Test
    {
        public Test()
        {
            Questions = new List<Question>();
        }
        public Test(string filePath)
        {
            Questions = new List<Question>();
            LoadFromFile(filePath);
        }
        public Test(string filePath, string password)
        {
            Questions = new List<Question>();
            LoadFromFile(filePath);
        }

        public int LoadFromFile(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Test));
            FileStream fs = new FileStream(path, FileMode.Open);
            Test test = (Test)serializer.Deserialize(fs);

            this.Questions = test.Questions;
            this.Name = test.Name;
            this.Time = test.Time;
            this.Encrypted = test.Encrypted;
            this.Author = test.Author;
            Path = path;
            fs.Close();
            return 0;
        }
        public int Save(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Test));
            FileStream fs = new FileStream(path, FileMode.Create);
            serializer.Serialize(fs, this);
            fs.Close();
            return 0;
        }
        public int Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Test));
            FileStream fs = new FileStream(Path, FileMode.Create);
            serializer.Serialize(fs, this);
            fs.Close();
            return 0;
        }
        public int Grade()
        {
            long grade = 0;
            foreach (Question q in Questions) // Loop through all the questions
            {
                int errors = 0;
                foreach (Answer a in q.answers) // Loop through all the answers
                {
                    if (a.Right != a.Selected) errors++;
                }
                if (errors == 0) grade++;
            }
            int ass = Questions.Count / 5;
            if (grade < (Questions.Count / 5)) return 1;
            if (grade < (Questions.Count / 4)) return 2;
            if (grade < (Questions.Count / 3)) return 3;
            if (grade < (Questions.Count)) return 4;
            return 5;
        }
    }



}
    