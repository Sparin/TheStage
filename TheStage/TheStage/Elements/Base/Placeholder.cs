using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace TheStage.Elements.Base
{
    //WIP
    class Placeholder
    {
        public Path Figure { get; private set; }

        public DoubleAnimation Animation { get; private set; }

        public PrimitiveType Type { get; private set; }
        
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
            Figure = new Path();
            Figure.Data = (Geometry)Figure.FindResource(Type.ToString("G"));
            Figure.StrokeThickness = 5;
            Figure.Opacity = 0;
            Figure.Fill = new SolidColorBrush(Color.FromRgb(43,27,23)); //Midnight
            Figure.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            DropShadowEffect ds = new DropShadowEffect();
            ds.Color = Color.FromRgb(255, 255, 255);
            ds.ShadowDepth = 0;
            Figure.Effect = ds;
        }

    }
}
