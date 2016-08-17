using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TheStage.Elements.Base;
using TheStage.Input;

namespace TheStage.Elements
{
    class HoldElement : Element
    {
        internal TimeSpan Duration { get; private set; }
        internal Status FirstStatus { get; private set; }
        internal TextBlock TextElement { get; private set; }

        internal Storyboard TimerAnimation { get; private set; }

        internal bool IsKeyDownSuccess { get; set; }
        internal bool IsKeyUpSuccess { get; set; }

        internal override bool IsPassed
        {
            get
            {
                return IsKeyUpSuccess && IsKeyDownSuccess;
            }
        }

        public HoldElement(KeyType type, Placeholder placeholder, Status status, Primitive primitive, TimeSpan duration) : base(type, placeholder, status, primitive)
        {
            Duration = duration;
            FirstStatus = new Status(status.PointOfAppearing);
            TextElement = new TextBlock()
            {
                Text = duration.TotalSeconds.ToString(),
                Width = 75,
                Height = 30,
                FontSize = 26,
                TextAlignment = System.Windows.TextAlignment.Center
            };

            AnimationSetup();
        }

        public void Tst()
        {
            Canvas.SetLeft(TextElement, Canvas.GetLeft(Placeholder.Figure));
            Canvas.SetTop(TextElement, Canvas.GetTop(Placeholder.Figure) + 70);
        }

        private void AnimationSetup()
        {
            TimerAnimation = new Storyboard();
            StringAnimationUsingKeyFrames animation = new StringAnimationUsingKeyFrames();
            animation.KeyFrames = GetFrameCollection(Duration.TotalSeconds);
            animation.Duration = Duration;
            //[0]; [1] - Position animation
            animation.BeginTime = Primitive.Animation.Children[0].BeginTime + Primitive.Animation.Children[0].Duration.TimeSpan;
            Storyboard.SetTargetProperty(animation, new PropertyPath(TextBlock.TextProperty));
            TimerAnimation.Children.Add(animation);
        }

        StringKeyFrameCollection GetFrameCollection(double duration)
        {
            StringKeyFrameCollection result = new StringKeyFrameCollection();

            for (; duration >= 0; duration -= 0.1)
                result.Add(new DiscreteStringKeyFrame(String.Format("{0:0.0}", duration)));

            return result;
        }
    }
}
