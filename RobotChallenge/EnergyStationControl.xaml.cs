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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotChallenge
{
    /// <summary>
    /// Interaction logic for EnergyStationControl.xaml
    /// </summary>
    public partial class EnergyStationControl : UserControl
    {
        public EnergyStationControl()
        {
            InitializeComponent();
            
            
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
    }
}
