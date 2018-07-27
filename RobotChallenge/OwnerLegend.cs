using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Robot.Tournament;

namespace RobotChallenge
{
    public class OwnerLegend:OwnerStatistics
    {
        public SolidColorBrush Color { get; set; }
        public string Name { get; set; }
        public OwnerLegend(OwnerStatistics stat)
        {
            TotalEnergy = stat.TotalEnergy;
            RobotsCount = stat.RobotsCount;
            Name = stat.Owner.Name;
            Color = new SolidColorBrush( ColorsFactory.OwnerColors[stat.Owner]);
        }
    }
}
