using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Robot.Common
{
    public delegate void RobotStepCompletedEventHandler(object sender, UpdateViewAfterRobotStepEventArgs e);

    public abstract class RobotCommand
    {
        public abstract UpdateViewAfterRobotStepEventArgs ChangeModel(IList<Robot> robots, int currentIndex, Map map);
        public void Apply(IList<Robot> robots, int currentIndex, Map map)
        {
            InvokeRobotStepCompleted(ChangeModel(robots, currentIndex, map));
        }

        public event RobotStepCompletedEventHandler RobotStepCompleted;

        public void InvokeRobotStepCompleted(UpdateViewAfterRobotStepEventArgs e)
        {
            RobotStepCompletedEventHandler handler = RobotStepCompleted;
            if (handler != null) handler(this, e);
        }

        public string Description { get; set; }
    }





    public class UpdateViewAfterRobotStepEventArgs:EventArgs
    {
        public string OwnerName;
        public Position RobotPosition;
        public Position NewRobotPosition;
        //Father+ son energy change - just for update view currently are not used
        public int TotalEnergyChange;


        public List<Position> MovedFrom;
        public List<Position> MovedTo;
    }


    

    
}
