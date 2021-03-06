﻿using System;
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
using TheStage.Elements.Base.Factories;

namespace TheStage.Elements.Base
{
    //WIP
    class Placeholder
    {
        public Image Figure { get; private set; }

        public Storyboard Animation { get; private set; }

        public PrimitiveType Type { get; private set; }

        public Placeholder(ElementFactory factory, PrimitiveType type, TimeSpan beginTime)
        {
            Type = type;
                        
            FigureSetup(factory);
            AnimationSetup(beginTime);
        }

        private void AnimationSetup(TimeSpan beginTime)
        {
            Animation = new Storyboard();
            DoubleAnimation d = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            d.BeginTime = beginTime;
            Storyboard.SetTargetProperty(d, new PropertyPath(Image.OpacityProperty));
            Animation.Children.Add(d);
        }

        private void FigureSetup(ElementFactory factory)
        {
            Figure = new Image();
            Figure.Source = factory.Placeholders[Type];
            Figure.Opacity = 0;
            DropShadowEffect ds = new DropShadowEffect();
            ds.Color = Color.FromRgb(0, 0, 0);
            ds.ShadowDepth = 0;
            Figure.Effect = ds;
        }

    }
}
