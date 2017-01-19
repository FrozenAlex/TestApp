using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TestApp.Methods
{
    [XmlInclude(typeof(ImageQuestion))]
    [XmlInclude(typeof(TextQuestion))]
    public class Question
    {
        public Question(){}
        [XmlArray]
        public List<Answer> answers { get; set; }

        [XmlAttribute]
        public int Cost { get; set; }
    }
    public class TextQuestion : Question// Text question
    {
        public TextQuestion() { }
        [XmlAttribute]
        public string Text { get; set; }
    }
    public class ImageQuestion : Question// Text question
    {
        public ImageQuestion() { }
        [XmlAttribute]
        public byte[] Image { get; set; }
        [XmlAttribute]
        public string Caption { get; set; }
    }
}
