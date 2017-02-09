using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TestApp.Methods;

namespace TestApp.Models
{
    [XmlRoot("Test")]  // Установка имени корнегого элемента
    [XmlInclude(typeof(Test))]  // Определение типа сериализации
    public partial class Test  // Набор вопросов
    {
        [XmlIgnore] // Игнорируем свойство при сериализации
        public string Path; // Путь к тесту
        [XmlIgnore]
        private string Password { get; set; } // Пароль к тесту. Используется только один раз) 
        [XmlAttribute]
        public ulong Time { get; set; } // Время на тест в секундах
        [XmlAttribute]
        public string Name { get; set; }  // Название теста
        [XmlAttribute]
        public string Author { get; set; }
        [XmlAttribute]
        public bool Encrypted { get; set; }

        [XmlArray("questions")] // Установка имени массива в XML 
        [XmlArrayItem("question")] // Установка названия элемента массива в XML
        public List<Question> Questions { get; set; } // Список вопросов
        public Test() { }

        public int Load(string path) // 
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Test)); // Создание сериализатора
            FileStream fs = new FileStream(path, FileMode.Open); // Создание потока файла
            Test test;
            test = (Test)serializer.Deserialize(fs);
            this.Questions = test.Questions;
            this.Name = test.Name;
            this.Time = test.Time;
            this.Author = test.Author;
            Path = path;
            fs.Close();
            return 0;

        }
        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Test)); // Создание сериализатора
            FileStream fs = new FileStream(this.Path, FileMode.Create); // Открытие потока на создание файла
            serializer.Serialize(fs, this);
            fs.Close();
        }
        public void Save(string path)
        {
            this.Path = path;
            Save();
        }
        public float Grade()
        {
            int max = Questions.Count; // Все вопросы по одному очку
            float score = 0; // Кол-во очков
            foreach (Question q in Questions) // Пробираю через все вопросы
            {
                var result = q.Grade();
                if (result == 1) score++;
            }
            if (score == 0) return 0;
            return ((max / 100) * score);
        }
    }


    [XmlInclude(typeof(Question.Edit))]
    [XmlInclude(typeof(Question.Radio))]
    [XmlInclude(typeof(Question.Select))]
    public class Question
    {
        public Question() { }
        /// <summary>
        /// Каждый вопрос оценивает себя сам и возвращает баллы
        /// </summary>
        /// <returns>Возвращает -1 - если не отвечен, 0 - если отвечен и 1 - если верен</returns>
        public virtual sbyte Grade() // каждый вопрос может себя оценить)
        {
            return 0;
        }
        public string Text { get; set; } // Текст вопроса
        public string Image { get; set; } // Путь к изображению
        
        public class Edit : Question
        {

            public Edit() { }
            public override sbyte Grade()
            {
                if (wrote == null || wrote == "")
                {
                    return -1;
                }
                else if (wrote.ToUpper() == answer.ToUpper()) // Игнорируем заглавные/строчные буквы
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            public string answer { get; set; }
            [XmlIgnore]
            public string wrote { get; set; }
        }

        public class Radio : Question
        {

            public Radio() { Selected = -1; }
            public override sbyte Grade()
            {
                if (Selected == -1)
                {
                    return -1;
                }
                else if (Answers[Selected].Right == true)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            [XmlIgnore]
            private int selected;

            public int Selected
            {
                get { return selected; }
                set
                {
                    selected = value;
                }
            }
            public List<Answer> Answers { get; set; } // Список ответов
        }

        public class Select : Question // Вопрос с галочками
        {

            public Select() { }
            public override sbyte Grade()
            {
                int Error = 0;
                foreach (Answer a in Answers)
                {
                    if (a.Right != a.Selected) Error++;
                }
                if (Error == 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            public List<Answer> Answers { get; set; } // Список ответов
        }
        public class Answer
        {
            public Answer() { }
            public Answer(string text, bool right = false)
            {
                Text = text;
                Right = right;
            }
            [XmlIgnore]
            public bool Selected { get; set; } // Лень думать
            public string Text { get; set; }
            public bool Right { get; set; }
        }

    }
}



