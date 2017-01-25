using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace TestApp.Methods
{
   

    /// <summary>
    /// Обьект тест 
    /// </summary>
    [XmlRoot("Test")]  // Установка имени корнегого элемента
    [XmlInclude(typeof(Test))]  // Определение типа сериализации
    public partial class Test  // Набор вопросов
    {
        const string defaultPassword = "Ass";


        [XmlIgnore] // Игнорируем свойство при сериализации
        private string Path; // Путь к тесту
        [XmlIgnore]
        private string Password { get; set; } // Пароль к тесту. Используется только один раз) 
        [XmlAttribute] // Аттрибут (<test аттрибут = 1>)
        public ulong Id { get; set; } // Идентификатор
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
        public List<Question> Questions; // Список вопросов


        public Test()
        {
            Questions = new List<Question>();  // Инициализируем пустой список ответов
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

        public int LoadFromFile(string path, string password = "Ass") // 
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Test)); // Создание сериализатора
            FileStream fs = new FileStream(path, FileMode.Open); // Создание потока файла
            var bytes = new byte[fs.Length];  // Создание массива байтов с размером равным размеру файла
            fs.Read(bytes, 0, (int)fs.Length); // Копирование из файла в переменную bytes
            var decriptedBytes = Crypt.Decrypt(bytes, password); // Расшифровка байтов 
            var memoryStream = new MemoryStream(); // Создание потока в память
            memoryStream.Write(decriptedBytes, 0, decriptedBytes.Length); // Запись в поток расшифрованных байтов
            memoryStream.Seek(0, SeekOrigin.Begin); // перевод указателя на первый байт в потоке
            Test test = (Test)serializer.Deserialize(memoryStream); // Десериализация расшифрованного теста из потока и импорт переменных
            this.Password = password;
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
            XmlSerializer serializer = new XmlSerializer(typeof(Test)); // Создание сериализатора
            FileStream fs = new FileStream(path, FileMode.Create); // Открытие потока на создание файла
            var memoryStream = new MemoryStream(); // Создание потока памяти
            serializer.Serialize(memoryStream, this); // Сериализация в поток 
            memoryStream.Seek(0, SeekOrigin.Begin); // Переход на первый байт в потоке
            var bytes = new byte[memoryStream.Length]; 
            memoryStream.Read(bytes, 0, (int)memoryStream.Length); // Чтение байтов из потока
            if (this.Password == null) this.Password = defaultPassword;
            var encryptedBytes = Crypt.Encrypt(bytes, this.Password); // Шифрование байтов
            fs.Write(encryptedBytes, 0, encryptedBytes.Length); // Запись в файл)
            fs.Close(); // Закрытие файла
            return 0;
        }
        public int Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Test));
            FileStream fs = new FileStream(this.Path, FileMode.Create);
            var memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, this);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var bytes = new byte[memoryStream.Length];
            memoryStream.Read(bytes, 0, (int)memoryStream.Length);

            var encryptedBytes = Crypt.Encrypt(bytes, this.Password);
            fs.Write(encryptedBytes, 0, encryptedBytes.Length);
            fs.Close();
            return 0;
        }
        public float Grade()
        {
            float max = 0; // Максимальное кол-во очков
            float score = 0; // Кол-во очков
            foreach (Question q in Questions) // Пробираю через все вопросы
            {
                max += q.Cost; // Подсчет максимума баллов
                int errors = 0;
                if (q.Answers.Count == 1) if (q.Answers[0].Written == q.Answers[0].Text) errors++;
                if (q.Answers.Count > 1)
                {
                    foreach (Answer a in q.Answers) {
                        if (a.Right != a.Selected) errors++;
                    }
                } 
                if (errors == 0) score += q.Cost;
            }
            if (score == 0) return 0;
            return ((max / 100) * score);
        } 
    }

    /// <summary>
    /// 
    /// </summary>
    [XmlInclude(typeof(ImageQuestion))]
    [XmlInclude(typeof(TextQuestion))]
    public class Question
    {
        public Question() { }
        [XmlArray]
        public List<Answer> Answers { get; set; } // Список ответов
        [XmlAttribute]
        public int Cost { get; set; } // Стоимость вопроса
        public string Text { get; set; } // Текст вопроса
    }
    public class TextQuestion : Question // Текстовый вопрос
    {
        public TextQuestion() { }
    }
    public class ImageQuestion : Question // Картиночный вопрос
    {
        public ImageQuestion() { }
        [XmlAttribute]
        public byte[] Image { get; set; }
    }

    public class Answer
    {
        public Answer() { }
        [XmlIgnore]
        public bool Selected { get; set; }
        [XmlIgnore]
        public string Written { get; set; } // Чтото страшное)
        public string Text { get; set; }
        public bool Right { get; set; }
    }
}
    