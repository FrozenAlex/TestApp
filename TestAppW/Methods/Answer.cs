using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TestApp.Methods
{
    public class Answer
    {
        public Answer() { }
        [XmlIgnore]
        public bool Selected { get; set; }
        public string Text { get; set; }
        public bool Right { get; set; }
    }
}
