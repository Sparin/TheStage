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
                TextAlignment = System.Windows.TextAlignment.Center,
                Opacity = 0
            };

            AnimationSetup();
        }

        public void Tst()
        {
            Canvas.SetLeft(TextElement, Canvas.GetLeft(Placeholder.Figure));
            Canvas.SetTop(TextElement, Canvas.GetTop(Placeholder.Figure) + 67.5);
        }

        private void AnimationSetup()
        {   
            TimerAnimation = new Storyboard();

            DoubleAnimation opacity = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            opacity.BeginTime = Placeholder.Animation.Children[0].BeginTime;
            Storyboard.SetTargetProperty(opacity, new PropertyPath(TextBlock.OpacityProperty));

            StringAnimationUsingKeyFrames animation = new StringAnimationUsingKeyFrames();
            animation.KeyFrames = GetFrameCollection(Duration.TotalMilliseconds);
            animation.Duration = Duration +TimeSpan.FromMilliseconds(150);
            //[0]; [1] - Position animation
            animation.BeginTime = Primitive.Animation.Children[0].BeginTime + Primitive.Animation.Children[0].Duration.TimeSpan;
            Storyboard.SetTargetProperty(animation, new PropertyPath(TextBlock.TextProperty));
            TimerAnimation.Children.Add(animation);
            TimerAnimation.Children.Add(opacity);
        }

        StringKeyFrameCollection GetFrameCollection(double duration)
        {
            StringKeyFrameCollection result = new StringKeyFrameCollection();

            for (int i = Convert.ToInt32(duration); i>= -100; i -= 100)
                result.Add(new DiscreteStringKeyFrame(String.Format("{0:0.0}", Convert.ToDouble(i)/1000)));
            result.Add(new DiscreteStringKeyFrame("-0.15"));
            
            return result;
        }
    }
}
