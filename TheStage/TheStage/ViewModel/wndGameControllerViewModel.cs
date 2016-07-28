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
using System.IO;

using ShapePath = System.Windows.Shapes.Path;
using System.Text.RegularExpressions;

namespace TheStage.ViewModel
{
    class wndGameControllerViewModel : INotifyPropertyChanged
    {
        //WIP
        //TODO: Make a new class with placeholder, primitive, animations and status references 
        private List<ShapePath> placeholders = new List<ShapePath>();
        private List<ShapePath> primitives = new List<ShapePath>();
        private List<Storyboard> animations = new List<Storyboard>();


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

        public wndGameControllerViewModel(string levelDirectory)
        {
            GameObjects = new ObservableCollection<UIElement>();

            if (!Directory.Exists(levelDirectory))
                throw new DirectoryNotFoundException(levelDirectory);
            string videoPath = Directory.GetFiles(levelDirectory, "video.*", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (videoPath == string.Empty)
                throw new FileNotFoundException("video.* not found");
            if (!File.Exists(levelDirectory + "/map.csv"))
                throw new FileNotFoundException("map.csv not found");

            mediaPlayer.Source = new Uri(videoPath, UriKind.Absolute);
            mediaPlayer.LoadedBehavior = MediaState.Manual;
            mediaPlayer.Stretch = Stretch.UniformToFill;

            GameObjects.Add(mediaPlayer);

            //TODO: Get Dimensions of media file
            Width = 1280;
            Height = 720;

            //Legacy too
            GestureClickedCommand = new Command(GestureClicked);

            ReadMap(levelDirectory + "/map.csv");

            mediaPlayer.Play();
            for (int i = 0; i < primitives.Count; i++)
                primitives[i].BeginStoryboard(animations[i]);


            #region Sample
            //MediaElement me = new MediaElement();
            //me.Source = new Uri(@"C:\Users\spari\Desktop\14692643770240.webm", UriKind.Absolute);
            //me.LoadedBehavior = MediaState.Manual;
            //me.Stretch = Stretch.UniformToFill;
            //me.Width = Width;
            //me.Height = Height;
            //me.Loaded += (e, s) =>
            //{
            //    me.Play();
            //};

            //Path basicSquare = new Path();
            //basicSquare.Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            //basicSquare.Data = (Geometry)basicSquare.FindResource("Square");

            //Path basicPlaceholder = new Path();
            //basicPlaceholder.Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            //basicPlaceholder.Fill = basicPlaceholder.Stroke;
            //basicPlaceholder.Data = (Geometry)basicSquare.FindResource("Square");

            //Canvas.SetLeft(basicSquare, -100);
            //Canvas.SetTop(basicSquare, -100);

            //Canvas.SetLeft(basicPlaceholder, 1000);
            //Canvas.SetTop(basicPlaceholder, 500);

            //PathGeometry way = new PathGeometry();
            //way.AddGeometry(Geometry.Parse("M -100 -100 C-100 -100 100 500 1000 500"));

            #region Animation Sample
            //Storyboard basicAnimation = new Storyboard();

            //DoubleAnimationUsingPath dx = new DoubleAnimationUsingPath();
            //dx.PathGeometry = way;
            //dx.BeginTime = TimeSpan.FromSeconds(2);
            //dx.Duration = TimeSpan.FromSeconds(4);
            //dx.Source = PathAnimationSource.X;
            //Storyboard.SetTargetProperty(dx, new PropertyPath(Canvas.LeftProperty));

            //DoubleAnimationUsingPath dy = new DoubleAnimationUsingPath();
            //dy.PathGeometry = way;
            //dy.BeginTime = TimeSpan.FromSeconds(2);
            //dy.Duration = TimeSpan.FromSeconds(4);
            //dy.Source = PathAnimationSource.Y;
            //Storyboard.SetTargetProperty(dy, new PropertyPath(Canvas.TopProperty));

            //basicAnimation.Children.Add(dx);
            //basicAnimation.Children.Add(dy);
            #endregion

            //basicAnimation.Completed += (s, e) =>
            //{
            //    GameObjects.Remove(basicPlaceholder);
            //    GameObjects.Remove(basicSquare);
            //    GameObjects.Add(basicStatus);
            //};


            //basicStatus = new TextBlock();
            //basicStatus.Text = "Bad";
            //basicStatus.FontSize = 24;
            //basicStatus.FontFamily = new FontFamily("Impact");
            //basicStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            //Canvas.SetLeft(basicStatus, 1025);
            //Canvas.SetTop(basicStatus, 500);

            //Storyboard statusStory = new Storyboard();
            //DoubleAnimation basicAnimationStatusOpacity = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            //DoubleAnimation basicAnimationStatusTop = new DoubleAnimation(500, 400, TimeSpan.FromSeconds(1));
            //Storyboard.SetTargetProperty(basicAnimationStatusTop, new PropertyPath(Canvas.TopProperty));
            //Storyboard.SetTargetProperty(basicAnimationStatusOpacity, new PropertyPath(Control.OpacityProperty));

            //statusStory.Children.Add(basicAnimationStatusTop);
            //statusStory.Children.Add(basicAnimationStatusOpacity);
            //statusStory.Completed += (s, e) => GameObjects.Remove(basicStatus);
            //basicStatus.Loaded += (s, e) =>
            //{
            //    basicStatus.BeginStoryboard(statusStory);
            //    //basicStatus.BeginAnimation(Control.OpacityProperty, basicAnimationStatusOpacity);
            //    //basicStatus.BeginAnimation(Canvas.TopProperty, basicAnimationStatusTop);
            //};


            //basicSquare.Loaded += (s, e) => basicSquare.BeginStoryboard(basicAnimation);
            //GameObjects.Add(me);
            //GameObjects.Add(basicPlaceholder);
            //GameObjects.Add(basicSquare);
            #endregion
        }

        //TODO: Create styles for the game objects
        private void ReadMap(string path)
        {
            string[] objects = File.ReadAllLines(path);
            for (int i = 0; i < objects.Length; i++)
            {
                string[] arguments = objects[i].Split(';');
                ShapePath gameObject = new ShapePath();
                gameObject.Data = (Geometry)gameObject.FindResource(arguments[0]);
                gameObject.Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 0));

                ShapePath gamePlaceholder = new ShapePath();
                gamePlaceholder.Data = gameObject.Data;
                gamePlaceholder.Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                gamePlaceholder.Fill = gamePlaceholder.Stroke;
                gamePlaceholder.StrokeThickness = 5;

                //Get and set start point and end point from the path
                MatchCollection points = Regex.Matches(arguments[1], @"(-?\d* *, *-?\d*)");
                Point startPoint = Point.Parse(points[0].Value);
                Point endPoint = Point.Parse(points[points.Count - 1].Value);

                Canvas.SetLeft(gameObject, startPoint.X);
                Canvas.SetTop(gameObject, startPoint.Y);

                Canvas.SetLeft(gamePlaceholder, endPoint.X);
                Canvas.SetTop(gamePlaceholder, endPoint.Y);

                #region Status of the game object 
                Storyboard gameStatusStory = GetStoryboardForStatus(endPoint);
                TextBlock gameStatus = new TextBlock();
                gameStatus.Text = "BAD";
                gameStatus.FontSize = 24;
                gameStatus.FontFamily = new FontFamily("Impact");
                gameStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                gameStatus.Loaded += (s, e) => gameStatus.BeginStoryboard(gameStatusStory);
                gameStatusStory.Completed += (s, e) => GameObjects.Remove(gameStatus);

                Canvas.SetLeft(gameStatus, endPoint.X-25);
                Canvas.SetTop(gameStatus, endPoint.Y);
                #endregion

                int beginTime = int.Parse(arguments[2]);
                int duration = int.Parse(arguments[3]);
                Storyboard gameStoryboard = GetStoryboardByPath(arguments[1], TimeSpan.FromMilliseconds(beginTime), TimeSpan.FromMilliseconds(duration));
                gameStoryboard.Completed += (s, e) =>
                {
                    GameObjects.Remove(gamePlaceholder);
                    GameObjects.Remove(gameObject);
                    primitives.Remove(gameObject);
                    placeholders.Remove(gamePlaceholder);
                    GameObjects.Add(gameStatus);                    
                };

                animations.Add(gameStoryboard);
                primitives.Add(gameObject);
                placeholders.Add(gamePlaceholder);

                GameObjects.Add(gamePlaceholder);
                GameObjects.Add(gameObject);
            }
        }

        private Storyboard GetStoryboardForStatus(Point endPoint)
        {
            Storyboard animation = new Storyboard();
            DoubleAnimation dOpacity = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            DoubleAnimation dTop = new DoubleAnimation(endPoint.Y, endPoint.Y-100, TimeSpan.FromSeconds(1));
            Storyboard.SetTargetProperty(dTop, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTargetProperty(dOpacity, new PropertyPath(Control.OpacityProperty));

            animation.Children.Add(dTop);
            animation.Children.Add(dOpacity);

            return animation;
        }
        private Storyboard GetStoryboardByPath(string way, TimeSpan beginTime, TimeSpan duration)
        {
            Storyboard animation = new Storyboard();

            PathGeometry path = new PathGeometry();
            path.AddGeometry(Geometry.Parse(way));

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

            animation.Children.Add(dx);
            animation.Children.Add(dy);

            return animation;
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
                        ShapePath nextObject = (ShapePath)GameObjects[1];
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
