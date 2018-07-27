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
using System.Windows.Shapes;

namespace RobotChallenge
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            buttonStart.IsEnabled = false;
        }

        private int variant = 0;

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            var w = new MainWindow ();
            w.InitializeChellange(variant);
            w.Show();
            this.Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            variant = 1;
            buttonStart.IsEnabled = true;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            variant = 2;
            buttonStart.IsEnabled = true;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            variant = 3;
            buttonStart.IsEnabled = true;
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            variant = 4;
            buttonStart.IsEnabled = true;
        }

        private void RadioButton_Checked_4(object sender, RoutedEventArgs e)
        {
            variant = 5;
            buttonStart.IsEnabled = true;
        }

        private void RadioButton_Checked_5(object sender, RoutedEventArgs e)
        {
            variant = 6;
            buttonStart.IsEnabled = true;
        }

        private void RadioButton_Checked_6(object sender, RoutedEventArgs e)
        {
            variant = 7;
            buttonStart.IsEnabled = true;
        }

        private void RadioButton_Checked_7(object sender, RoutedEventArgs e)
        {
            variant = 8;
            buttonStart.IsEnabled = true;
        }
    }
}
