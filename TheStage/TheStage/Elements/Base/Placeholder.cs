using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace TheStage.Elements.Base
{
    //WIP
    class Placeholder
    {
        public Viewbox Figure { get; private set; }

        public DoubleAnimation Animation { get; private set; }

        public PrimitiveType Type { get; private set; }

        static public ResourceDictionary Geometries { get; private set; }

        static Placeholder()
        {
            Geometries = new ResourceDictionary();
            Geometries.Source = new Uri("Resources/Geometries/PlaceholderGeometry.xaml", UriKind.RelativeOrAbsolute);
        }

        public Placeholder(PrimitiveType type, TimeSpan beginTime)
        {
            Type = type;

            AnimationSetup(beginTime);
            FigureSetup();
        }

        private void AnimationSetup(TimeSpan beginTime)
        {
            Animation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            Animation.BeginTime = beginTime;
        }

        private void FigureSetup()
        {
            Figure = new Viewbox();
            Figure = Geometries[Type.ToString("G")] as Viewbox;
            Figure.Opacity = 0;
            DropShadowEffect ds = new DropShadowEffect();
            ds.Color = Color.FromRgb(0, 0, 0);
            ds.ShadowDepth = 0;
            Figure.Effect = ds;
        }

    }
}
