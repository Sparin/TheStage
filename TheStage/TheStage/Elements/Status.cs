using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace TheStage.Elements
{
    public class Status
    {
        public Point PointOfAppearing { get; set; }
        public Storyboard Animation { get; private set; }
        public TextBlock TextElement { get; set;}

        public Status(Point pointOfAppearing)
        {
            PointOfAppearing = pointOfAppearing;
            TextElement = new TextBlock();
            TextElement.Style = (Style)TextElement.FindResource("StatusMainStyle");
            TextElement.Style = (Style)TextElement.FindResource("StatusMissStyle");

            Canvas.SetLeft(TextElement, pointOfAppearing.X);
            Canvas.SetTop(TextElement, pointOfAppearing.Y+10);

            AnimationSetup();
        }

        private void AnimationSetup()
        {
            Animation = new Storyboard();

            DoubleAnimation dOpacity = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(2));
            DoubleAnimation dTop = new DoubleAnimation(PointOfAppearing.Y, PointOfAppearing.Y - 100, TimeSpan.FromSeconds(2));
            Storyboard.SetTargetProperty(dTop, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTargetProperty(dOpacity, new PropertyPath(Control.OpacityProperty));

            Animation.Children.Add(dTop);
            Animation.Children.Add(dOpacity);

            TextElement.Loaded += (s, e) => TextElement.BeginStoryboard(Animation);
        }
    }
}
