using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TheStage.Elements.Base
{
    public class Primitive
    {
        public Geometry WayOfAnimation { get; set; }
        public Viewbox Figure { get; private set; }
        public PrimitiveType Type { get; private set; }
        public Storyboard Animation { get; private set; }

        static public ResourceDictionary Geometries { get; private set; }

        static Primitive()
        {
            Geometries = new ResourceDictionary();
            Geometries.Source = new Uri("Resources/Geometries/PrimitiveGeometry.xaml", UriKind.RelativeOrAbsolute);
        }

        public Primitive(PrimitiveType type, string wayOfAnimation, TimeSpan beginTime, TimeSpan duration) :
            this(type, Geometry.Parse(wayOfAnimation), beginTime, duration)
        { }

        public Primitive(PrimitiveType type, Geometry wayOfAnimation, TimeSpan beginTime, TimeSpan duration)
        {
            WayOfAnimation = wayOfAnimation;
            Type = type;

            FigureSetup(type);
            AnimationSetup(beginTime, duration);
        }

        private void FigureSetup(PrimitiveType type)
        {
            Figure = new Viewbox();
            Figure = Geometries[Type.ToString("G")] as Viewbox;
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
