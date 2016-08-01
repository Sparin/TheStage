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
using TheStage.Input;
using TheStage.Elements;

namespace TheStage.ViewModel
{
    class wndGameControllerViewModel : INotifyPropertyChanged
    {
        //WIP
        //TODO: Make a new class with placeholder, primitive, animations and status references 
        private List<Element> Elements = new List<Element>();

        private MediaElement mediaPlayer = new MediaElement();

        private InputMap leftInputMap;
        public InputMap LeftInputMap { get { return leftInputMap; } set { leftInputMap = value; RaisePropertyChanged(); } }

        private InputMap rightInputMap;
        public InputMap RightInputMap { get { return rightInputMap; } set { rightInputMap = value; RaisePropertyChanged(); } }

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

        public Command KeyClickedCommand { get; private set; }
        
        public wndGameControllerViewModel(string levelDirectory)
        {
            GameObjects = new ObservableCollection<UIElement>();

            LeftInputMap = new InputMap();
            LeftInputMap.Bottom = Key.S;
            LeftInputMap.Left = Key.A;
            LeftInputMap.Top = Key.W;
            leftInputMap.Right = Key.D;

            if (!Directory.Exists(levelDirectory))
                throw new DirectoryNotFoundException(levelDirectory);
            string videoPath = Directory.GetFiles(levelDirectory, "video.*", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (videoPath == string.Empty)
                throw new FileNotFoundException("video.* not found");
            if (!File.Exists(levelDirectory + "/map.csv"))
                throw new FileNotFoundException("map.csv not found");

            //TODO: Get Dimensions of media file
            Width = 1920;
            Height = 1080;

            mediaPlayer.Source = new Uri(videoPath, UriKind.Absolute);
            mediaPlayer.LoadedBehavior = MediaState.Manual;
            mediaPlayer.Stretch = Stretch.UniformToFill;
            mediaPlayer.Width = width;
            mediaPlayer.Height = height;

            GameObjects.Add(mediaPlayer);

            KeyClickedCommand = new Command(KeyClicked);

            ReadMap(levelDirectory + "/map.csv");

            //TODO: Synchronization logic            
            mediaPlayer.Play();

            var placeholders = Elements.Select((x) => x.Placeholder).ToList();
            for (int i = 0; i < placeholders.Count; i++)
                placeholders[i].Figure.BeginAnimation(Control.OpacityProperty, placeholders[i].Animation);

            var primitives = Elements.Select((x) => x.Primitive).ToList();
            for (int i = 0; i < primitives.Count; i++)
                primitives[i].Animation.Begin(primitives[i].Figure, true);
        }

        //TODO: Create styles for the game objects
        private void ReadMap(string path)
        {
            string[] objects = File.ReadAllLines(path);
            for (int i = 0; i < objects.Length; i++)
            {
                string[] arguments = objects[i].Split(';');

                //Get and set start point and end point from the path
                MatchCollection points = Regex.Matches(arguments[1], @"(-?\d* *, *-?\d*)");
                Point startPoint = Point.Parse(points[0].Value);
                Point endPoint = Point.Parse(points[points.Count - 1].Value);
                int beginTime = int.Parse(arguments[2]);
                int duration = int.Parse(arguments[3]);

                PrimitiveType type = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), arguments[0]);
                Placeholder placeholder = new Placeholder(type, TimeSpan.FromMilliseconds(beginTime));
                Status status = new Status(endPoint);
                Primitive primitive = new Primitive(type, arguments[1], TimeSpan.FromMilliseconds(beginTime), TimeSpan.FromMilliseconds(duration));

                Element element = new Element(KeyType.Bottom, placeholder, status, primitive);

                Canvas.SetLeft(primitive.Figure, startPoint.X);
                Canvas.SetTop(primitive.Figure, startPoint.Y);
                Canvas.SetLeft(placeholder.Figure, endPoint.X);
                Canvas.SetTop(placeholder.Figure, endPoint.Y);

                status.Animation.Completed += (s, e) => GameObjects.Remove(status.TextElement);

                primitive.Animation.Completed += (s, e) =>
                {
                    GameObjects.Add(status.TextElement);
                    GameObjects.Remove(placeholder.Figure);
                    GameObjects.Remove(primitive.Figure);
                    Elements.Remove(element);
                };

                GameObjects.Add(placeholder.Figure);
                GameObjects.Add(primitive.Figure);
                Elements.Add(element);
            }
        }

        //TODO: epsilon(!) beginTime includes duration
        //      calculating epsilon between two points is too long (kek, welcome to sync)
        public void KeyClicked(object keyType)
        {
            string key = keyType as string;
            if (key == null)
                throw new ArgumentNullException("keyType");

            KeyType type = (KeyType)Enum.Parse(typeof(KeyType), key);
            if (Elements.Count == 0 || Elements[0].Key != type)
                return;

            Elements[0].Primitive.Animation.GetCurrentProgress(Elements[0].Primitive.Figure);
            double? distance = Elements[0].Primitive.Animation.GetCurrentProgress(Elements[0].Primitive.Figure);
            //Math.Sqrt(
            //Math.Pow(Canvas.GetLeft(Elements[0].Primitive.Figure) - Canvas.GetLeft(Elements[0].Placeholder.Figure), 2) +
            //                   Math.Pow(Canvas.GetTop(Elements[0].Primitive.Figure) - Canvas.GetTop(Elements[0].Placeholder.Figure), 2));
            double epsilon = 0.9;//100;
            Console.WriteLine(distance);

            if (distance > epsilon)
            {
                Elements[0].Status.TextElement.Style = (Style)Elements[0].Status.TextElement.FindResource("StatusGoodStyle");
                Score += 100;
            }
            else
                Elements[0].Status.TextElement.Style = (Style)Elements[0].Status.TextElement.FindResource("StatusBadStyle");

            if (Elements[0].Primitive.Animation.GetCurrentState(Elements[0].Primitive.Figure) == ClockState.Active)
                Elements[0].Primitive.Animation.SkipToFill(Elements[0].Primitive.Figure);
        }

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
