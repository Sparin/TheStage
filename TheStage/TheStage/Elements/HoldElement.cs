using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheStage.Elements.Base;
using TheStage.Input;

namespace TheStage.Elements
{
    class HoldElement : Element
    {
        internal TimeSpan Duration { get; private set; }
        internal Primitive PrimitiveFollower { get; private set; }

        public HoldElement(KeyType type, Placeholder placeholder, Status status, Primitive primitive, TimeSpan duration) : base(type, placeholder, status, primitive)
        {
            Duration = duration;
            PrimitiveFollower = new Primitive(primitive.Type, primitive.WayOfAnimation, (TimeSpan)primitive.Animation.Children[0].BeginTime + Duration, primitive.Animation.Children[0].Duration.TimeSpan);
        }
    }
}
