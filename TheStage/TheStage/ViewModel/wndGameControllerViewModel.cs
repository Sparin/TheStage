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
using TheStage.Elements.Base;

namespace TheStage.ViewModel
{
    class wndGameControllerViewModel : INotifyPropertyChanged
    {
        const int POST_ANIMATION_MILLISECONDS = 150;

        private List<Element> Elements = new List<Element>();

        private MediaElement mediaPlayer = new MediaElement();

        private InputMap leftInputMap;
        public InputMap LeftInputMap { get { return leftInputMap; } set { leftInputMap = value; RaisePropertyChanged(); } }

        private InputMap rightInputMap;
        public InputMap RightInputMap { get { return rightInputMap; } set { rightInputMap = value; RaisePropertyChanged(); } }

        public ObservableCollection<UIElement> GameObjects { get; private set; }

        #region Properties
        private bool isHaltMode;
        public bool IsHaltMode
        {
            get { return isHaltMode; }
            set { isHaltMode = value; RaisePropertyChanged(); }
        }

        private long comboMultiplyer = 1;
        public long ComboMultiplyer
        {
            get { return comboMultiplyer; }
            set { comboMultiplyer = value; RaisePropertyChanged(); }
        }

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

        public wndGameControllerViewModel(string levelDirectory)
        {
            GameObjects = new ObservableCollection<UIElement>();

            LeftInputMap = new InputMap();
            LeftInputMap.Bottom = Key.S;
            LeftInputMap.Left = Key.A;
            LeftInputMap.Top = Key.W;
            LeftInputMap.Right = Key.D;

            RightInputMap = new InputMap();
            RightInputMap.Bottom = Key.K;
            RightInputMap.Left = Key.J;
            RightInputMap.Top = Key.I;
            RightInputMap.Right = Key.L;

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

            ReadMap(levelDirectory + "/map.csv");

            //TODO: Synchronization logic            
            mediaPlayer.Play();

            var placeholders = Elements.Select((x) => x.Placeholder).ToList();
            for (int i = 0; i < placeholders.Count; i++)
                placeholders[i].Figure.BeginAnimation(Control.OpacityProperty, placeholders[i].Animation);

            var primitives = Elements.Select((x) => x.Primitive).ToList();
            for (int i = 0; i < primitives.Count; i++)
                primitives[i].Animation.Begin(primitives[i].Figure, true);

            var followers = Elements.Select((x) =>
            {
                if (x is HoldElement)
                    return ((HoldElement)x).PrimitiveFollower;
                return null;
            }).Where((x) => x != null)
            .ToList();
            for (int i = 0; i < followers.Count; i++)
                followers[i].Animation.Begin(followers[i].Figure, true);
        }

        private void ReadMap(string path)
        {
            string[] objects = File.ReadAllLines(path);
            for (int i = 0; i < objects.Length; i++)
            {
                string[] arguments = objects[i].Split(';');

                //Get and set start point and end point from the path
                MatchCollection points = Regex.Matches(arguments[2], @"(-?\d* *, *-?\d*)");
                Point startPoint = Point.Parse(points[0].Value);
                Point endPoint = Point.Parse(points[points.Count - 1].Value);
                TimeSpan beginTime = TimeSpan.FromMilliseconds(int.Parse(arguments[3]));
                TimeSpan duration = TimeSpan.FromMilliseconds(int.Parse(arguments[4]) + 150);
                Geometry way = Geometry.Parse(arguments[2]);

                PrimitiveType type = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), arguments[1]);
                Placeholder placeholder = new Placeholder(type, beginTime);
                Status status = new Status(endPoint);
                Primitive primitive = new Primitive(type, way, beginTime, duration);

                Element element;
                switch (arguments[0])
                {
                    case "SingleElement":
                        element = new SingleElement((KeyType)type, placeholder, status, primitive);
                        break;
                    case "DoubleElement":
                        element = new DoubleElement((KeyType)type, placeholder, status, primitive);
                        break;
                    case "HoldElement":
                        element = new HoldElement((KeyType)type, placeholder, status, primitive, TimeSpan.FromMilliseconds(int.Parse(arguments[5])));
                        break;
                    default:
                        throw new ArgumentException("Element Type: " + arguments[0]);
                }

                Point placeholderPosition = new Point();
                Point tgPlaceholder = new Point();
                way.GetFlattenedPathGeometry().GetPointAtFractionLength(1 - TimeSpan.FromMilliseconds(150).TotalMilliseconds / duration.TotalMilliseconds, out placeholderPosition, out tgPlaceholder);

                Canvas.SetLeft(primitive.Figure, startPoint.X);
                Canvas.SetTop(primitive.Figure, startPoint.Y);
                Canvas.SetLeft(placeholder.Figure, placeholderPosition.X);
                Canvas.SetTop(placeholder.Figure, placeholderPosition.Y);

                status.Animation.Completed += (s, e) => GameObjects.Remove(status.TextElement);

                if (element is HoldElement)
                    ((HoldElement)element).PrimitiveFollower.Animation.Completed += (s, e) =>
                    {
                        GameObjects.Add(status.TextElement);
                        GameObjects.Remove(placeholder.Figure);
                        GameObjects.Remove(primitive.Figure);
                        GameObjects.Remove(((HoldElement)element).PrimitiveFollower.Figure);
                        Elements.Remove(element);
                    };
                else
                    primitive.Animation.Completed += (s, e) =>
                    {
                        GameObjects.Add(status.TextElement);
                        GameObjects.Remove(placeholder.Figure);
                        GameObjects.Remove(primitive.Figure);
                        Elements.Remove(element);
                    };

                GameObjects.Add(placeholder.Figure);
                GameObjects.Add(primitive.Figure);
                if (element is HoldElement)
                    GameObjects.Add(((HoldElement)element).PrimitiveFollower.Figure);
                Elements.Add(element);
            }
        }

        public void KeyDown(object sender, KeyEventArgs args)
        {
            InputMap map = GetInputMap(args.Key);
            if (Elements.Count == 0 || map == null || isHaltMode)
                return;

            if (Elements[0] is HoldElement)
                isHaltMode = true;

            bool isRightPad = true;
            if (map == LeftInputMap)
                isRightPad = false;

            TimeSpan currentTime = Elements[0].Primitive.Animation.GetCurrentTime(Elements[0].Primitive.Figure).Value;
            TimeSpan duration = Elements[0].Primitive.Animation.Children[0].Duration.TimeSpan + Elements[0].Primitive.Animation.Children[0].BeginTime.Value;
            TimeSpan diff = duration - currentTime;
            TimeSpan epsilon = TimeSpan.FromMilliseconds(200 + POST_ANIMATION_MILLISECONDS);

            KeyType key = GetDirection(map, args.Key);
            if (diff < epsilon && Elements[0].Key == key)
                SetElementCondition(Elements[0], isRightPad);

            if (Elements[0].IsPassed)
            {
                int qualityBeat = 0;
                if (diff.Milliseconds >= POST_ANIMATION_MILLISECONDS)
                    qualityBeat = (diff.Milliseconds - POST_ANIMATION_MILLISECONDS) / 25;
                else
                    qualityBeat = Math.Abs(diff.Milliseconds - POST_ANIMATION_MILLISECONDS) / 25;

                switch (qualityBeat)
                {
                    case 0://x < 50ms
                    case 1:
                        Elements[0].Status.TextElement.Style = (Style)Elements[0].Status.TextElement.FindResource("StatusExcellentStyle");
                        Score += (100 * ComboMultiplyer);
                        ComboMultiplyer++;
                        break;
                    case 2://x < 75ms 
                        Elements[0].Status.TextElement.Style = (Style)Elements[0].Status.TextElement.FindResource("StatusGoodStyle");
                        Score += (75 * ComboMultiplyer);
                        ComboMultiplyer++;
                        break;
                    case 3://x < 100ms
                        Elements[0].Status.TextElement.Style = (Style)Elements[0].Status.TextElement.FindResource("StatusSafeStyle");
                        Score += 50;
                        ComboMultiplyer = 1;
                        break;
                    case 4://x < 150ms
                    case 5:
                        Elements[0].Status.TextElement.Style = (Style)Elements[0].Status.TextElement.FindResource("StatusAwfulStyle");
                        Score += 10;
                        ComboMultiplyer = 1;
                        break;
                    case 6://x < 200ms
                    case 7:
                        Elements[0].Status.TextElement.Style = (Style)Elements[0].Status.TextElement.FindResource("StatusBadStyle");
                        ComboMultiplyer = 1;
                        break;

                }
                if (Elements[0].Primitive.Animation.GetCurrentState(Elements[0].Primitive.Figure) == ClockState.Active)
                    Elements[0].Primitive.Animation.SkipToFill(Elements[0].Primitive.Figure);
            }
        }

        public void KeyUp(object sender, KeyEventArgs args)
        {
            isHaltMode = false;
        }

        private InputMap GetInputMap(Key key)
        {
            if (LeftInputMap.Contains(key))
                return LeftInputMap;
            if (RightInputMap.Contains(key))
                return RightInputMap;
            return null;
        }

        private KeyType GetDirection(InputMap map, Key key)
        {
            for (int i = 1; i < 5; i++)
                if (map[i] == key)
                    return (KeyType)i;
            return KeyType.None;
        }

        private void SetElementCondition(Element element, bool isRightPad)
        {
            Type type = element.GetType();
            switch (type.Name)
            {
                case "SingleElement":
                    ((SingleElement)element).IsPressed = true;
                    break;
                case "DoubleElement":
                    DoubleElement local = (DoubleElement)element;
                    if (isRightPad)
                        local.RightPadPressed = true;
                    else
                        local.LeftPadPressed = true;
                    break;
                default:
                    throw new NotImplementedException("Element Type: " + element.GetType());
            }
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
