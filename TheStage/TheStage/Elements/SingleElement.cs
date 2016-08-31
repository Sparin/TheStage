using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheStage.Elements.Base;
using TheStage.Input;

namespace TheStage.Elements
{
    class SingleElement:Element
    {
        internal bool IsPressed { get; set; }

        internal override bool IsPassed
        {
            get
            {
                return IsPressed;
            }
        }

        public SingleElement(KeyType type, Placeholder placeholder, Status status, Primitive primitive) : base(type, placeholder, status, primitive)
        { }
    }
}
