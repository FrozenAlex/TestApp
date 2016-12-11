using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Security;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace TestModel
{
    [XmlRoot("Test")]
    [XmlInclude(typeof(Test))]
    public class Test  // One set of questions
    {
        [XmlAttribute] public ulong id; // Id for some reason
        [XmlAttribute] public ulong time;  // Time limit in seconds. 0 - No limit
        [XmlAttribute] public string name; // Name of the test
        [XmlAttribute] public string author;  // Author
        [XmlAttribute] public bool encrypted;

        [XmlArray("questions")]
        [XmlArrayItem("question")]
        public List<Question> questions;

        public Test()
        {
            questions = new List<Question>();
        }
        public Test(string filePath)
        {
            questions = new List<Question>();
            LoadFromFile(filePath);
        }
        public Test(string filePath, string password)
        {
            questions = new List<Question>();
            LoadFromFile(filePath);
        }

        public int LoadFromFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Test));
            FileStream fs = new FileStream(filename, FileMode.Open);
            Test test = (Test)serializer.Deserialize(fs);

            this.questions = test.questions;
            this.name = test.name;
            this.time = test.time;
            this.encrypted = test.encrypted;
            this.author = test.author;

            fs.Close();
            return 0;
        }
        public int SaveToFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Test));
            FileStream fs = new FileStream(filename, FileMode.Create);
            serializer.Serialize(fs, this);
            fs.Close();
            return 0;
        }

        public int Grade()
        {
            long grade = 0;
            foreach (Question q in questions) // Loop through all the questions
            {
                int errors = 0;
                foreach (Answer a in q.answers) // Loop through all the answers
                {
                    if (a.isRight != a.selected) errors++;
                }
                if (errors == 0) grade++;
            }
            int ass = questions.Count / 5;
            if (grade < (questions.Count / 5)) return 1;
            if (grade < (questions.Count / 4)) return 2;
            if (grade < (questions.Count / 3)) return 3;
            if (grade < (questions.Count)) return 4;
            return 5;
        }
    }


    public class Answer
    {
        // Ignored fields for operation
        [XmlIgnore]
        public bool selected { get; set; }

        public string text { get; set; }
        public bool isRight { get; set; }


        public Answer(string Text, bool Right)
        {
            text = Text;
            isRight = Right;
        }
        public Answer()
        {
       
        }
    }

    public class Question // Individual question
    {
        [XmlAttribute] public string type;
        [XmlAttribute] public string text;
        [XmlAttribute] public byte[] image;

        [XmlArray] public List<Answer> answers;

        public Question()
        {
            type = "string";
            answers = new List<Answer>();
        }
        public Question(string questionText)
        {
            text = questionText;
            type = "string";
            answers = new List<Answer>();
        }

    }
}
