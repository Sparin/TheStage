﻿using System;
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
using TheStage.Elements.Base.Factories;
using TheStage.Controls;
using TheStage.Controls.GameController;

namespace TheStage.ViewModel
{
    class wndGameControllerViewModel : INotifyPropertyChanged
    {
        const int POST_ANIMATION_MILLISECONDS = 150;

        private Dictionary<Storyboard, FrameworkElement> Animations = new Dictionary<Storyboard, FrameworkElement>();
        private List<Element> Elements = new List<Element>();
        private MediaElementSL mediaPlayer = new MediaElementSL();
        public ObservableCollection<UIElement> GameObjects { get; private set; }

        Timer preStartTimer;
        PauseMenu pauseMenu;
        Statistics statsControl;

        private InputMap leftInputMap;
        public InputMap LeftInputMap { get { return leftInputMap; } set { leftInputMap = value; RaisePropertyChanged(); } }

        private InputMap rightInputMap;
        public InputMap RightInputMap { get { return rightInputMap; } set { rightInputMap = value; RaisePropertyChanged(); } }

        public Command PauseCommand { get { return new Command(Pause); } }

        #region Properties
        private bool isHoldOn = false;
        public bool IsHoldOn
        {
            get { return isHoldOn; }
            set { isHoldOn = value; RaisePropertyChanged(); }
        }

        private long comboMultiplyer = 1;
        public long ComboMultiplyer
        {
            get { return comboMultiplyer; }
            set { comboMultiplyer = value; RaisePropertyChanged(); }
        }

        private StatisticsViewModel stats = new StatisticsViewModel();
        public StatisticsViewModel Stats
        {
            get { return stats; }
            set { stats = value; RaisePropertyChanged(); }
        }

        public string LevelDirectory { get; private set; }

        private double width = 0;
        public double Width
        {
            get { return width; }
            set { width = value; RaisePropertyChanged(); }
        }

        private double height = 0;
        public double Height
        {
            get { return height; }
            set { height = value; RaisePropertyChanged(); }
        }
        #endregion

        public wndGameControllerViewModel(string levelDirectory)
        {
            LevelDirectory = levelDirectory;

            //TODO: Sync input map with setting via menu
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

            Load();

            Play();
        }

        private void InitializeUI()
        {
            Image avatarHolder = new Image() { Width = 230, Stretch = Stretch.UniformToFill };
            avatarHolder.Source = ElementFactory.GetBitmapSource(@"Resources\Images\UI\GameController\avatarHolder.png");
            Canvas.SetBottom(avatarHolder, 0);
            Canvas.SetRight(avatarHolder, 0);

            Image jukebox = new Image() { Width = 273, Stretch = Stretch.UniformToFill };
            jukebox.Source = ElementFactory.GetBitmapSource(@"Resources\Images\UI\GameController\jukebox.png");
            Canvas.SetBottom(jukebox, 0);

            ImageBrush chain = new ImageBrush(ElementFactory.GetBitmapSource(@"Resources\Images\UI\GameController\chain.png")) { TileMode = TileMode.FlipX };
            chain.Viewport = new Rect(0, 0, 100, 50);
            chain.ViewportUnits = BrushMappingMode.Absolute;
            chain.Stretch = Stretch.Uniform;
            Canvas c = new Canvas() { Width = this.Width - jukebox.Width - avatarHolder.Width, Height = 50, Background = chain };
            Canvas.SetBottom(c, 0);
            Canvas.SetLeft(c, jukebox.Width);

            Image marker = new Image();
            marker.Source = ElementFactory.GetBitmapSource(@"Resources\Images\UI\GameController\marker.png");
            Canvas.SetBottom(marker, 0);
            Storyboard markerAnimation = new Storyboard();
            DoubleAnimation markerLeft = new DoubleAnimation(jukebox.Width, c.Width + jukebox.Width - marker.Source.Width, mediaPlayer.NaturalDuration);
            Storyboard.SetTargetProperty(markerAnimation, new PropertyPath(Canvas.LeftProperty));
            markerAnimation.Children.Add(markerLeft);            
            marker.Loaded += (s, e) => { markerAnimation.Begin(marker, HandoffBehavior.Compose, true);
                markerAnimation.Pause(marker); Animations.Add(markerAnimation, marker); };

            GameObjects.Add(c);
            GameObjects.Add(marker);
            GameObjects.Add(jukebox);
            GameObjects.Add(avatarHolder);
        }

        private Button CreateButton(string content, Action clickDelegate)
        {
            Button result = new Button();
            result.Height = 50;
            result.Width = 100;
            result.Content = content;
            result.Click += (s, e) => clickDelegate();

            return result;
        }

        private void Load()
        {
            if (GameObjects != null)
                GameObjects.Clear();
            else
                GameObjects = new ObservableCollection<UIElement>();
            Elements = new List<Element>();
            mediaPlayer = new MediaElementSL();

            if (!Directory.Exists(LevelDirectory))
                throw new DirectoryNotFoundException(LevelDirectory);
            string videoPath = Directory.GetFiles(LevelDirectory, "video.*", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (videoPath == string.Empty)
                throw new FileNotFoundException("video.* not found");
            if (!File.Exists(LevelDirectory + "/map.csv"))
                throw new FileNotFoundException("map.csv not found");

            mediaPlayer.Source = new Uri(videoPath, UriKind.Absolute);
            mediaPlayer.LoadedBehavior = MediaState.Manual;
            mediaPlayer.Stretch = Stretch.UniformToFill;
            mediaPlayer.MediaOpened += (s, e) =>
            {
                Width = mediaPlayer.NaturalVideoWidth;
                Height = mediaPlayer.NaturalVideoHeight;
                Pause(true);
                preStartTimer = new Timer() { Width = width, Height = height };
                preStartTimer.Completed += () => { GameObjects.Remove(preStartTimer); Resume(); };

                InitializeUI();
                GameObjects.Add(preStartTimer);
            };
            mediaPlayer.MediaEnded += (s, e) =>
            {
                statsControl = new Statistics(Stats) { Width = this.Width, Height = this.Height };
                statsControl.Restart += () => { GameObjects.Remove(statsControl); Stop(); Play(); };
                statsControl.BackToMenu += () => { throw new NotImplementedException(); };
                GameObjects.Add(statsControl);
            };

            GameObjects.Add(mediaPlayer);

            ReadMap(LevelDirectory + "/map.csv");

#if DEBUG
            Button btnPlay = CreateButton("Play", Play);
            GameObjects.Add(btnPlay);
            Button btnResume = CreateButton("Resume", Resume);
            Canvas.SetTop(btnResume, 50);
            GameObjects.Add(btnResume);
            Button btnPause = CreateButton("Pause", Pause);
            Canvas.SetTop(btnPause, 100);
            GameObjects.Add(btnPause);
            Button btnStop = CreateButton("Stop", Stop);
            Canvas.SetTop(btnStop, 150);
            GameObjects.Add(btnStop);
#endif
        }

        #region Interaction cotrol of animations
        private void Play()
        {
            if (mediaPlayer.CurrentState != MediaElementState.Stopped && mediaPlayer.CurrentState != MediaElementState.Closed)
                return;
            Animations = new Dictionary<Storyboard, FrameworkElement>();
            Stats = new StatisticsViewModel();
            mediaPlayer.Play();

            var placeholders = Elements.Select((x) => { Animations.Add(x.Placeholder.Animation, x.Placeholder.Figure); return x.Placeholder; }).ToList();
            for (int i = 0; i < placeholders.Count; i++)
                placeholders[i].Animation.Begin(placeholders[i].Figure, HandoffBehavior.Compose, true);

            var primitives = Elements.Select((x) => { Animations.Add(x.Primitive.Animation, x.Primitive.Figure); return x.Primitive; }).ToList();
            for (int i = 0; i < primitives.Count; i++)
                primitives[i].Animation.Begin(primitives[i].Figure, HandoffBehavior.Compose, true);

            var holdElements = Elements.Select((x) => x as HoldElement).Where((x) => x is HoldElement).ToList();
            for (int i = 0; i < holdElements.Count; i++)
            {
                Animations.Add(holdElements[i].TimerAnimation, holdElements[i].TextElement);
                holdElements[i].TimerAnimation.Begin(holdElements[i].TextElement, HandoffBehavior.Compose, true);
            }
        }

        private void Resume()
        {
            mediaPlayer.Play();
            foreach (KeyValuePair<Storyboard, FrameworkElement> pair in Animations)
                pair.Key.Resume(pair.Value);
        }

        private void Stop()
        {
            mediaPlayer.Stop();
            foreach (KeyValuePair<Storyboard, FrameworkElement> pair in Animations)
                pair.Key.Stop(pair.Value);
            Load();
        }

        private void Pause() { Pause(false); }
        private void Pause(bool isControllable)
        {
            if (GameObjects.Contains(preStartTimer))
                return;
            if (GameObjects.Contains(pauseMenu))
            {
                GameObjects.Remove(pauseMenu); Resume(); return;
            }

            mediaPlayer.Pause();
            foreach (KeyValuePair<Storyboard, FrameworkElement> pair in Animations)
                pair.Key.Pause(pair.Value);

            if (!isControllable)
            {
                pauseMenu = new PauseMenu() { DataContext = Stats, Width = width, Height = height };
                pauseMenu.Resume += () => { GameObjects.Remove(pauseMenu); Resume(); };
                pauseMenu.Restart += () => { GameObjects.Remove(pauseMenu); Stop(); Play(); };
                pauseMenu.BackToMenu += () => { throw new NotImplementedException(); };
                GameObjects.Add(pauseMenu);
            }
        }
        #endregion

        private void ReadMap(string path)
        {
            SingleFactory singleFactory = new SingleFactory();
            HoldFactory holdFactory = new HoldFactory();
            DoubleFactory doubleFactory = new DoubleFactory();

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
                Status status = new Status(endPoint);
                Placeholder placeholder;
                Primitive primitive;

                Element element;
                switch (arguments[0])
                {
                    case "SingleElement":
                        placeholder = new Placeholder(singleFactory, type, beginTime);
                        primitive = new Primitive(singleFactory, type, way, beginTime, duration);
                        element = new SingleElement((KeyType)type, placeholder, status, primitive);
                        break;
                    case "DoubleElement":
                        placeholder = new Placeholder(doubleFactory, type, beginTime);
                        primitive = new Primitive(doubleFactory, type, way, beginTime, duration);
                        element = new DoubleElement((KeyType)type, placeholder, status, primitive);
                        break;
                    case "HoldElement":
                        placeholder = new Placeholder(holdFactory, type, beginTime);
                        primitive = new Primitive(holdFactory, type, way, beginTime, duration);
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

                if (element is HoldElement)
                {
                    HoldElement el = element as HoldElement;
                    el.Tst();
                    //[0]; [1] index is position animation
                    element.Primitive.Animation.Children[0].Completed += (s, e) =>
                    {
                        GameObjects.Add(el.FirstStatus.TextElement);
                        GameObjects.Remove(primitive.Figure);
                        if (!el.IsKeyDownSuccess)
                            if (el.TimerAnimation.GetCurrentState(el.TextElement) == ClockState.Active)
                                el.TimerAnimation.SkipToFill(el.TextElement);
                    };
                    el.TimerAnimation.Completed += (s, e) =>
                    {
                        GameObjects.Add(status.TextElement);
                        GameObjects.Remove(placeholder.Figure);
                        GameObjects.Remove(el.TextElement);
                        Elements.Remove(element);
                        if (element.Status.TextElement.Style == (Style)element.Status.TextElement.FindResource("StatusMissStyle"))
                            InvalidateQuality(null, -1);
                    };
                }
                else
                {
                    status.Animation.Completed += (s, e) => GameObjects.Remove(status.TextElement);

                    primitive.Animation.Completed += (s, e) =>
                    {
                        GameObjects.Add(status.TextElement);
                        GameObjects.Remove(placeholder.Figure);
                        GameObjects.Remove(primitive.Figure);
                        Elements.Remove(element);
                        if (element.Status.TextElement.Style == (Style)element.Status.TextElement.FindResource("StatusMissStyle"))
                            InvalidateQuality(null, -1);
                    };
                }

                GameObjects.Add(placeholder.Figure);
                GameObjects.Add(primitive.Figure);
                if (element is HoldElement)
                    GameObjects.Add(((HoldElement)element).TextElement);
                Elements.Add(element);
            }
        }

        public void KeyDown(object sender, KeyEventArgs args)
        {
            InputMap map = GetInputMap(args.Key);
            if (Elements.Count == 0 || map == null || isHoldOn || GameObjects.Contains(pauseMenu))
                return;

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

            if (Elements[0].IsPassed || (Elements[0] is HoldElement && Elements[0].Key == key))
            {
                int qualityBeat = 0;
                if (diff.Milliseconds >= POST_ANIMATION_MILLISECONDS)
                    qualityBeat = (diff.Milliseconds - POST_ANIMATION_MILLISECONDS) / 25;
                else
                    qualityBeat = Math.Abs(diff.Milliseconds - POST_ANIMATION_MILLISECONDS) / 25;

                Status status = Elements[0].Status;
                if (Elements[0] is HoldElement)
                    status = ((HoldElement)Elements[0]).FirstStatus;
                if (Elements[0] is HoldElement || Elements[0] is SingleElement)
                    IsHoldOn = true;

                InvalidateQuality(status, qualityBeat);

                if (Elements[0].Primitive.Animation.GetCurrentState(Elements[0].Primitive.Figure) == ClockState.Active)
                    Elements[0].Primitive.Animation.SkipToFill(Elements[0].Primitive.Figure);
            }
        }

        public void KeyUp(object sender, KeyEventArgs args)
        {
            InputMap map = GetInputMap(args.Key);
            if (Elements.Count == 0 || map == null || GameObjects.Contains(pauseMenu))
                return;
            if (Elements[0] is HoldElement)
            {
                HoldElement element = Elements[0] as HoldElement;

                if (!element.IsKeyDownSuccess)
                    return;

                bool isRightPad = true;
                if (map == LeftInputMap)
                    isRightPad = false;

                TimeSpan currentTime = element.TimerAnimation.GetCurrentTime(element.TextElement).Value;
                TimeSpan duration = element.TimerAnimation.Children[0].Duration.TimeSpan + element.TimerAnimation.Children[0].BeginTime.Value;
                TimeSpan diff = duration - currentTime;
                TimeSpan epsilon = TimeSpan.FromMilliseconds(200 + POST_ANIMATION_MILLISECONDS);

                KeyType key = GetDirection(map, args.Key);
                if (diff < epsilon && Elements[0].Key == key)
                    SetElementCondition(Elements[0], isRightPad, true);

                if (Elements[0].IsPassed && diff < epsilon && Elements[0].Key == key)
                {
                    int qualityBeat = 0;
                    if (diff.Milliseconds >= POST_ANIMATION_MILLISECONDS)
                        qualityBeat = (diff.Milliseconds - POST_ANIMATION_MILLISECONDS) / 25;
                    else
                        qualityBeat = Math.Abs(diff.Milliseconds - POST_ANIMATION_MILLISECONDS) / 25;

                    Status status = Elements[0].Status;

                    InvalidateQuality(status, qualityBeat);
                }
                if (element.TimerAnimation.GetCurrentState(element.TextElement) == ClockState.Active && IsHoldOn)
                    element.TimerAnimation.SkipToFill(element.TextElement);
            }
            IsHoldOn = false;
        }

        private void InvalidateQuality(Status status, int qualityBeat)
        {
            switch (qualityBeat)
            {
                case 0://x < 50ms
                case 1:
                    status.TextElement.Style = (Style)status.TextElement.FindResource("StatusExcellentStyle");
                    Stats.Score += (100 * ComboMultiplyer);
                    Stats.Excellent++;
                    ComboMultiplyer++;
                    break;
                case 2://x < 75ms 
                    status.TextElement.Style = (Style)status.TextElement.FindResource("StatusGoodStyle");
                    Stats.Score += (75 * ComboMultiplyer);
                    Stats.Good++;
                    ComboMultiplyer++;
                    break;
                case 3://x < 100ms
                    status.TextElement.Style = (Style)status.TextElement.FindResource("StatusSafeStyle");
                    Stats.Score += 50;
                    Stats.Safe++;
                    ComboMultiplyer = 1;
                    break;
                case 4://x < 150ms
                case 5:
                    status.TextElement.Style = (Style)status.TextElement.FindResource("StatusAwfulStyle");
                    Stats.Score += 10;
                    Stats.Awful++;
                    ComboMultiplyer = 1;
                    break;
                case 6://x < 200ms
                case 7:
                    status.TextElement.Style = (Style)status.TextElement.FindResource("StatusBadStyle");
                    Stats.Bad++;
                    ComboMultiplyer = 1;
                    break;
                default:
                    Stats.Miss++;
                    ComboMultiplyer = 1;
                    break;
            }
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

        private void SetElementCondition(Element element, bool isRightPad, bool isKeyUp = false)
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
                case "HoldElement":
                    if (!isKeyUp)
                        ((HoldElement)element).IsKeyDownSuccess = true;
                    else
                        ((HoldElement)element).IsKeyUpSuccess = true;
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
