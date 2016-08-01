using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using TheStage.Elements;
using TheStage.Input;

namespace TheStage.Elements
{    
    class Element
    {
        public KeyType Key { get; private set; }
        
        public Placeholder Placeholder { get; private set; }        
        public Primitive Primitive { get; private set; }        
        public Status Status { get; private set; }
        
        public Element(KeyType type, Placeholder placeholder, Status status, Primitive primitive)
        {
            Placeholder = placeholder;
            Status = status;
            Primitive = primitive;
            Key = type;                        
        }
    }
}
