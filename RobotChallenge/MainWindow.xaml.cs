using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Robot.Common;
using Robot.Tournament;

namespace RobotChallenge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<Position, RobotControl> RobotPlace = new Dictionary<Position, RobotControl>();

        public IList<OwnerStatistics> RobotStatistics { get; set; }
        public int Variant { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            CreateGrid();

            Logger.OnLogRound += (e, args) =>
            { TextBlockRoundNumber.Text = args.Number.ToString(); BindStatistics(); };
            Logger.OnLogMessage += LogMessage;
            Messsages = new ObservableCollection<LogMessage>();
            LogList.ItemsSource = Messsages;

        }

        public ObservableCollection<LogMessage> Messsages { get; set; }
        private const int maxNumber = 13;
        public void LogMessage(object sender, LogEventArgs args)
        {


                                   var c = new SolidColorBrush(Colors.Red);
                                   var n = "";
                                   if (args.Owner != null)
                                   {
                                       c = new SolidColorBrush(ColorsFactory.OwnerColors[args.Owner]);
                                       n = args.Owner.Name;
                                   }

                                   if (IsLogVisisble())
                                   {
                                       Messsages.Insert(0, new LogMessage() { Color = c, Message = args.Message, Name = n });

                                       if (Messsages.Count > maxNumber)
                                           Messsages.RemoveAt(maxNumber);
                                   }

        }

        public void InitializeChellange(int variant)
        {
            _map = new Map(variant, ReflectionScanner.ScanLibs().Length*Runner.InitialRobotsCount);
            _runner = new Runner(_map, ModelChanged);
            ColorsFactory.Initialize(_runner.Owners);
            PaintRobots(_runner.Robots);
            PaintStations();
            this.Height = 1250;
            BindStatistics();
        }

        private void BindStatistics()
        {
            var st = _runner.CalculateOwnerStatistics();
            var list = new List<OwnerLegend>();

            foreach (var ownerStatistics in st)
            {
                list.Add(new OwnerLegend(ownerStatistics));
            }
            ListStatistics.ItemsSource = list;

        }

        private Robot.Tournament.Runner _runner;
        private Map _map;

        private const int CellWidth = 9;
        private const int CellCount = 100;

        private void PaintRobots(IList<Robot.Common.Robot> robots)
        {
            foreach (var robot in robots)
            {
                CreateRobot(robot.Owner, robot.Position);
            }

        }

        public void SetAnimation(RobotControl robotControl, Position from, Position to, bool isLast)
        {
            
            currentAnimation=new Storyboard();
            TimeSpan ts = TimeSpan.FromMilliseconds(_speed);

            currentAnimation.Children.Add(MoveLeftAnimation(robotControl, from.X * CellWidth, to.X * CellWidth, ts));
            currentAnimation.Children.Add(MoveRightAnimation(robotControl, from.Y * CellWidth, to.Y * CellWidth, ts));
            if (isLast)
                currentAnimation.Completed += ViewUpdatedChanged;
            currentAnimation.Begin();
        }

        public DoubleAnimation MoveLeftAnimation(RobotControl robotControl, double fromX, double toX, TimeSpan ts)
        {
            var animation = new DoubleAnimation { From = fromX, To = toX, Duration = ts};
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTarget(animation, robotControl);

            return animation;
        }

        public DoubleAnimation MoveRightAnimation(RobotControl robotControl, double fromX, double toX,  TimeSpan ts)
        {
            var animation = new DoubleAnimation { From = fromX, To = toX, Duration = ts};
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
            Storyboard.SetTarget(animation, robotControl);
            return animation;
        }

        private Storyboard currentAnimation;

        private Dictionary<Position, RobotControl> _positionChanges;
        /// <summary>
        /// Apply changes on the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ModelChanged(object sender, UpdateViewAfterRobotStepEventArgs args)
        {

            _positionChanges = new Dictionary<Position, RobotControl>();

            if (args.MovedFrom == null || args.MovedFrom.Count == 0)
            {
                if (args.NewRobotPosition != null)
                {
                    CreateRobot(args.Owner, args.NewRobotPosition);
                    var r = RobotPlace[args.NewRobotPosition];
                    currentAnimation = r.AnimateOpacity();
                    currentAnimation.Completed += new EventHandler(ViewUpdatedChanged);
                    currentAnimation.Begin();
                }
                else if (args.RobotPosition != null)
                {
                    
                    var r = RobotPlace[args.RobotPosition];
                    currentAnimation = r.AnimateOpacity();
                    currentAnimation.Completed += new EventHandler(ViewUpdatedChanged);
                    currentAnimation.Begin();
                }
            }

            else
            {

                for (int i = 0; i < args.MovedFrom.Count; i++)
                {
                    var from = args.MovedFrom[i];
                    var to = args.MovedTo[i];
                    var r = RobotPlace[from];

                    SetAnimation(r, from, to, i == args.MovedFrom.Count - 1);

                    RobotPlace.Remove(from);
                    _positionChanges.Add(to, r);
                }
            }
        }

        public void ViewUpdatedChanged(object sender, EventArgs args)
        {
            
            //remove data
            if (currentAnimation != null && currentAnimation.Children != null)
            currentAnimation.Children.Clear();
            currentAnimation = null;
            try
            {
                if (_positionChanges != null)
                {
                    foreach (var changePosition in _positionChanges.Keys)
                    {
                        RobotPlace.Add(changePosition, _positionChanges[changePosition]);
                    }
                    _positionChanges.Clear();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            _runner.DoStep();
        }

        private void PaintStations()
        {
            foreach (var station in _map.Stations)
            {
                var control = new EnergyStationControl();
                RobotGrid.Children.Add(control);
                SetPosition(control, station.Position);
            }
        }

        private void SetPosition(EnergyStationControl control, Position position)
        {
            control.Top = position.Y * CellWidth;
            control.Left = position.X * CellWidth;
        }

        private void SetPosition(RobotControl control, Position position)
        {
            control.Top = position.Y * CellWidth;
            control.Left = position.X * CellWidth;
        }

        private void CreateRobot(Owner owner, Position position)
        {
            var control = new RobotControl();
            control.BackgroundPanel.Fill = new SolidColorBrush(ColorsFactory.OwnerColors[owner]);
            RobotGrid.Children.Add(control);
            control.BackgroundPanel.Height = CellWidth;
            control.BackgroundPanel.Width = CellWidth;
            SetPosition(control, position);
            SetValue(Panel.ZIndexProperty, 0);
            RobotPlace.Add(position, control); 
        }

        private void CreateGrid()
        {

            for (int i = 0; i <= 100; i++)
            {

                RobotGrid.Children.Add(new Line()
                                           {
                                               X1 = i * CellWidth,
                                               X2 = i * CellWidth,
                                               Y1 = 0,
                                               Y2 = CellCount * CellWidth,
                                               StrokeThickness = 1,
                                               Stroke = new SolidColorBrush(Colors.Black)
                                           }
                    );

                RobotGrid.Children.Add(new Line()
                                           {
                                               Y1 = i * CellWidth,
                                               Y2 = i * CellWidth,
                                               X1 = 0,
                                               X2 = CellCount * CellWidth,
                                               StrokeThickness = 1,
                                               Stroke = new SolidColorBrush(Colors.Black)
                                           }
                    );
            }
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = false;
            ViewUpdatedChanged(null, null);
            
        }

        private static int _speed = 1500;
        public static int Speed { get { return _speed; } }
        private void comboBoxSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxSpeed.SelectedValue != null)
            _speed = int.Parse(comboBoxSpeed.SelectedValue.ToString());
        }

        bool IsLogVisisble()
        {
            return LogList.Visibility == Visibility.Visible;
        }

        private void buttonShowHideLog_Click(object sender, RoutedEventArgs e)
        {
            if (LogList.Visibility == Visibility.Hidden)
            {
                buttonShowHideLog.Content = "Hide log";
                LogList.Visibility = Visibility.Visible;
                LogList.ItemsSource = Messsages;
            }
            else
            {
                buttonShowHideLog.Content = "Show log";
                LogList.Visibility = Visibility.Hidden;
                LogList.ItemsSource = null;
                Messsages.Clear();



            }
        }

    }
}

