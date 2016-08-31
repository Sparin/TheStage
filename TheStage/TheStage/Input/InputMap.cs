using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TheStage.Input
{
    public class InputMap
    {
        public Key Top { get; set; }
        public Key Right { get; set; }
        public Key Left { get; set; }
        public Key Bottom { get; set; }

        public Key this[KeyType type]
        {
            get
            {
                switch (type)
                {
                    case KeyType.Top: return Top;
                    case KeyType.Right: return Right;
                    case KeyType.Left: return Left;
                    case KeyType.Bottom: return Bottom;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public Key this[int index]
        {
            get
            {
                return this[(KeyType)index];
            }
        }

        public bool Contains(Key key)
        {
            for (int i = 1; i < 5; i++)
                if (key == this[i])
                    return true;
            return false;
        }
    }
}
