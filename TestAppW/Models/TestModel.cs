﻿using System;
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
        [XmlIgnore]
        public List<Question> Wrong // Список неверных вопросов
        {
            get
            {
                List<Question> wrong = new List<Question>();
                foreach (Question q in Questions) // Пробираю через все вопросы
                    if (q.Grade() != 1) wrong.Add(q);
                return wrong;
            }
        }




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
                if (q.Grade() == 1) score++;      
            if (score == 0) return 0;
            return score / max;
        }
        
    }


    [XmlInclude(typeof(Question.Edit))]
    [XmlInclude(typeof(Question.Radio))]
    [XmlInclude(typeof(Question.Select))]
    public abstract class Question
    {
        // Свойства
        public string Text { get; set; } // Текст вопроса
        public string Image { get; set; } // Путь к изображению
        // Методы
        public abstract sbyte Grade(); // каждый вопрос может себя оценить)

        // Вопрос с текстовым ответом
        public class Edit : Question
        {
            // Свойства
            public string answer { get; set; }
            [XmlIgnore]
            public string wrote { get; set; }

            // Методы
            public override sbyte Grade()
            {
                if (String.IsNullOrEmpty(wrote)) return -1; // Если строка пустая то -1 (не отвечен)   
                if (String.Equals(wrote, answer, StringComparison.CurrentCultureIgnoreCase)) return 1; // Сравниваем обе строки без учета регистра
                return 0; // Если не нашлось совпадений то не верно
            }
           
        }

        // Вопрос с выбором одного правильного ответа
        public class Radio : Question
        {
            // Свойства
            [XmlIgnore]
            public int Selected { get; set; } = -1;
            public List<Answer> Answers { get; set; } // Список ответов

            // Методы
            public override sbyte Grade()
            {
                if (Selected == -1) return -1; // Если не выбран
                if (Answers[Selected].Right == true) return 1; // Если выбран правильный
                return 0; // Если не правильный
            }    
        }

        // Вопрос с галочками
        public class Select : Question // Вопрос с галочками
        {
            // Свойства
            public List<Answer> Answers { get; set; } // Список ответов

            // Методы
            public override sbyte Grade()
            {
                int Error = 0;
                foreach (Answer a in Answers)
                    if (a.Right != a.Selected) Error++;
                if (Error == 0) return 1;
                return 0;
            }
   
        }
        public class Answer
        {
            // Свойства
            [XmlIgnore]
            public bool Selected { get; set; } // Выбран ли ответ
            public string Text { get; set; } // Текст варианта ответа
            public bool Right { get; set; } // Правильность ответа
        }

    }
}



