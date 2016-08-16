using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using TheStage.Elements.Base;
using TheStage.Input;

namespace TheStage.Elements
{
    class HoldElement : Element
    {
        internal TimeSpan Duration { get; private set; }

        public HoldElement(KeyType type, Placeholder placeholder, Status status, Primitive primitive, TimeSpan duration) : base(type, placeholder, status, primitive)
        {
            Duration = duration;
        }
    }
}
