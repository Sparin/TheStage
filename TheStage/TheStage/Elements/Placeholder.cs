using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TheStage.Elements
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
            Figure.Fill = new SolidColorBrush(Color.FromScRgb(255, 0, 0, 0));
            Figure.Stroke = new SolidColorBrush(Color.FromScRgb(100, 255, 255, 255));
        }

    }
}
