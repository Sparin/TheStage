using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Apex.MVVM;

namespace TheStage.ViewModel
{
    class wndGameControllerViewModel : INotifyPropertyChanged
    {
        private Queue<Path> placeholders = new Queue<Path>();
        private Queue<Path> primitives = new Queue<Path>();
        private Queue<TextBlock> status = new Queue<TextBlock>();

        private MediaElement mediaPlayer = new MediaElement();

        public ObservableCollection<UIElement> GameObjects { get; private set; }

        #region Properties
        private long score = 0;
        public long Score
        {
            get { return score; }
            set { score = value; RaisePropertyChanged(); }
        }

        private double width;
        public double Width
        {
            get { return width; }
            set { width = value; RaisePropertyChanged(); }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set { height = value; RaisePropertyChanged(); }
        }
        #endregion

        #region TODO: Input map bindings 
        //private string testGesture = "W";
        //public string TestGesture
        //{
        //    get { return testGesture; }
        //    set { testGesture = value; RaisePropertyChanged(); }
        //}
        //private InputMap rigthPanel;
        //public InputMap RightPanel
        //{
        //    get { return rigthPanel; }
        //    set { rigthPanel = value; RaisePropertyChanged(); }
        //}
        #endregion

        public Command GestureClickedCommand { get; private set; }


        //private List<>
        TextBlock basicStatus;

        public wndGameControllerViewModel()
        {
            GameObjects = new ObservableCollection<UIElement>();
            Width = 1280;
            Height = 720;
            GestureClickedCommand = new Command(GestureClicked);


            MediaElement me = new MediaElement();
            me.Source = new Uri(@"C:\Users\spari\Desktop\14692643770240.webm", UriKind.Absolute);
            me.LoadedBehavior = MediaState.Manual;
            me.Stretch = Stretch.UniformToFill;
            me.Width = Width;
            me.Height = Height;
            me.Loaded += (e, s) =>
            {
                me.Play();
            };

            Path basicSquare = new Path();
            basicSquare.Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            basicSquare.Data = (Geometry)basicSquare.FindResource("Square");

            Path basicPlaceholder = new Path();
            basicPlaceholder.Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            basicPlaceholder.Fill = basicPlaceholder.Stroke;
            basicPlaceholder.Data = (Geometry)basicSquare.FindResource("Square");

            Canvas.SetLeft(basicSquare, -100);
            Canvas.SetTop(basicSquare, -100);

            Canvas.SetLeft(basicPlaceholder, 1000);
            Canvas.SetTop(basicPlaceholder, 500);

            PathGeometry way = new PathGeometry();
            way.AddGeometry(Geometry.Parse("M -100 -100 C-100 -100 100 500 1000 500"));

            #region Animation Sample
            Storyboard basicAnimation = new Storyboard();

            DoubleAnimationUsingPath dx = new DoubleAnimationUsingPath();
            dx.PathGeometry = way;
            dx.BeginTime = TimeSpan.FromSeconds(2);
            dx.Duration = TimeSpan.FromSeconds(4);
            dx.Source = PathAnimationSource.X;
            Storyboard.SetTargetProperty(dx, new PropertyPath(Canvas.LeftProperty));

            DoubleAnimationUsingPath dy = new DoubleAnimationUsingPath();
            dy.PathGeometry = way;
            dy.BeginTime = TimeSpan.FromSeconds(2);
            dy.Duration = TimeSpan.FromSeconds(4);
            dy.Source = PathAnimationSource.Y;
            Storyboard.SetTargetProperty(dy, new PropertyPath(Canvas.TopProperty));

            basicAnimation.Children.Add(dx);
            basicAnimation.Children.Add(dy);
            #endregion

            basicAnimation.Completed += (s, e) =>
            {
                GameObjects.Remove(basicPlaceholder);
                GameObjects.Remove(basicSquare);
                GameObjects.Add(basicStatus);
            };
            

            basicStatus = new TextBlock();
            basicStatus.Text = "Bad";
            basicStatus.FontSize = 24;
            basicStatus.FontFamily = new FontFamily("Impact");
            basicStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            Canvas.SetLeft(basicStatus, 1025);
            Canvas.SetTop(basicStatus, 500);

            Storyboard statusStory = new Storyboard();
            DoubleAnimation basicAnimationStatusOpacity = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            DoubleAnimation basicAnimationStatusTop = new DoubleAnimation(500, 400, TimeSpan.FromSeconds(1));
            Storyboard.SetTargetProperty(basicAnimationStatusTop, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTargetProperty(basicAnimationStatusOpacity, new PropertyPath(Control.OpacityProperty));

            statusStory.Children.Add(basicAnimationStatusTop);
            statusStory.Children.Add(basicAnimationStatusOpacity);
            statusStory.Completed += (s, e) => GameObjects.Remove(basicStatus);
            basicStatus.Loaded += (s, e) =>
            {
                basicStatus.BeginStoryboard(statusStory);
                //basicStatus.BeginAnimation(Control.OpacityProperty, basicAnimationStatusOpacity);
                //basicStatus.BeginAnimation(Canvas.TopProperty, basicAnimationStatusTop);
            };


            basicSquare.Loaded += (s, e) => basicSquare.BeginStoryboard(basicAnimation);
            GameObjects.Add(me);
            GameObjects.Add(basicPlaceholder);
            GameObjects.Add(basicSquare);


        }

        public void KeyClicked(object keyType)
        {
            string key = keyType as string;
            if (key == null)
                throw new ArgumentNullException("keyType");

            switch (key)
            {
                case "Left":

                    break;
                case "Top":

                    break;

                case "Right":

                    break;

                case "Bottom":

                    break;
            }
        }

        #region Legacy of sample
        public void GestureClicked(object gesture)
        {
            string gest = gesture as string;
            if (gest == null)
                throw new ArgumentNullException("gesture");

                if (GameObjects.Count > 2)
                    switch (gest)
                    {
                        case "Left":
                            Path nextObject = (Path)GameObjects[1];
                            double epsilon = 100;
                            if (nextObject.Data == (PathGeometry)nextObject.FindResource("Square"))
                            {
                                double distance = Math.Sqrt(
                                    Math.Pow(Canvas.GetLeft(GameObjects[1]) - Canvas.GetLeft(GameObjects[2]), 2) +
                                    Math.Pow(Canvas.GetTop(GameObjects[1]) - Canvas.GetTop(GameObjects[2]), 2));
                                if (distance < epsilon)
                                {
                                    basicStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                                    basicStatus.Text = "Good";
                                }
                                else
                                {

                                }
                                //else
                                //    MessageBox.Show("Bad");
                            }

                            break;
                        case "Top":

                            break;

                        case "Right":

                            break;

                        case "Bottom":

                            break;
                    }

            //MessageBox.Show(gest);
        }
        #endregion

        #region MVVM Related
        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
