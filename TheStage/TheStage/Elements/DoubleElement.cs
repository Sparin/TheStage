using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheStage.Input;

namespace TheStage.Elements
{
    class DoubleElement : Element
    {
        internal bool LeftPadPressed { get; set; }
        internal bool RightPadPressed { get; set; }

        internal override bool IsPassed
        {
            get
            {
                return LeftPadPressed && RightPadPressed;
            }
        }

        internal DoubleElement(KeyType type, Placeholder placeholder, Status status, Primitive primitive) : base(type, placeholder, status, primitive)
        { }
    }
}
