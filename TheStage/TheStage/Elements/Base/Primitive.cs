using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TheStage.Elements.Base.Factories;

namespace TheStage.Elements.Base
{
    public class Primitive
    {
        public Geometry WayOfAnimation { get; set; }
        public Image Figure { get; private set; }
        public PrimitiveType Type { get; private set; }
        public Storyboard Animation { get; private set; }

        public Primitive(ElementFactory factory,PrimitiveType type, string wayOfAnimation, TimeSpan beginTime, TimeSpan duration) :
            this(factory,type, Geometry.Parse(wayOfAnimation), beginTime, duration)
        { }

        public Primitive(ElementFactory factory,PrimitiveType type, Geometry wayOfAnimation, TimeSpan beginTime, TimeSpan duration)
        {
            WayOfAnimation = wayOfAnimation;
            Type = type;

            FigureSetup(factory);
            AnimationSetup(beginTime, duration);
        }

        private void FigureSetup(ElementFactory factory)
        {
            Figure = new Image();
            Figure.Source = factory.Primitives[Type];
        }

        private void AnimationSetup(TimeSpan beginTime, TimeSpan duration)
        {
            Animation = new Storyboard();

            PathGeometry path = new PathGeometry();
            path.AddGeometry(WayOfAnimation);

            DoubleAnimationUsingPath dx = new DoubleAnimationUsingPath();
            dx.PathGeometry = path;
            dx.BeginTime = beginTime;
            dx.Duration = duration;
            dx.Source = PathAnimationSource.X;
            Storyboard.SetTargetProperty(dx, new PropertyPath(Canvas.LeftProperty));

            DoubleAnimationUsingPath dy = new DoubleAnimationUsingPath();
            dy.PathGeometry = path;
            dy.BeginTime = beginTime;
            dy.Duration = duration;
            dy.Source = PathAnimationSource.Y;
            Storyboard.SetTargetProperty(dy, new PropertyPath(Canvas.TopProperty));

            Animation.Children.Add(dx);
            Animation.Children.Add(dy);
        }
    }

    public enum PrimitiveType
    {
        Square=1,
        Triangle=2,
        Circle=3,
        Cross=4
    }
}
