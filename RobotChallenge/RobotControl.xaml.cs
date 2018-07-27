using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotChallenge
{
    /// <summary>
    /// Interaction logic for RobotControl.xaml
    /// </summary>
    public partial class RobotControl : UserControl
    {
        public RobotControl()
        {
            InitializeComponent();
            SetValue(Canvas.ZIndexProperty, 1);

        }
        private double _left;
        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;

                Canvas.SetLeft(this, _left);
            }
        }

        private double _top;
        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                Canvas.SetTop(this, _top);
            }
        }

        public Storyboard AnimateOpacity()
        {
            //var myAnimatedBrush = BackgroundPanel.Fill;
            
            //this.RegisterName("MyAnimatedBrush", myAnimatedBrush);

            var opacityAnimation = new DoubleAnimation
                                       {
                                           From = 0.0,
                                           To = 100.0,
                                           Duration = TimeSpan.FromMilliseconds(MainWindow.Speed/6),
                                           AutoReverse = true
                                       };
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(SolidColorBrush.OpacityProperty));
            //Storyboard.SetTargetName(opacityAnimation, "MyAnimatedBrush");
           /* Storyboard.SetTargetProperty(
                opacityAnimation, new PropertyPath(SolidColorBrush.OpacityProperty));
            */



            Storyboard.SetTarget(opacityAnimation, BackgroundPanel);

            var storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);
            storyboard.AutoReverse = true;
            return storyboard;
        }
    }
}
