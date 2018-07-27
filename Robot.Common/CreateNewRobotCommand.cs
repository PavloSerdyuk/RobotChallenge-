using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robot.Common
{
    public sealed class CreateNewRobotCommand : RobotCommand
    {


        /// <summary>
        /// Default value for new robot energy
        /// </summary>
        private int _newRobotEnergy = 100;

        public int NewRobotEnergy
        {
            get { return _newRobotEnergy; }
            set { _newRobotEnergy = value; } 
        }
    

        public override UpdateViewAfterRobotStepEventArgs ChangeModel(IList<Robot> robots, int currentIndex, Map map)
        {
            var result = new UpdateViewAfterRobotStepEventArgs();
            var myRobot = robots[currentIndex];
            Robot newRobot = null;
            var energyLossToCreateNewRobot = Variant.GetInstance().EnergyLossToCreateNewRobot;
            var energyLoss = energyLossToCreateNewRobot + NewRobotEnergy;

            if (robots.Count(r => r.Owner.Name == myRobot.Owner.Name) >= 100)
            {
                Description = String.Format("FAILED: number of robots reached 100.");
            }

            if (myRobot.Energy > energyLoss)
            {
                var position = map.FindFreeCell(myRobot.Position, robots);
                newRobot = new Robot() { Position = position, Energy = NewRobotEnergy, Owner = myRobot.Owner };
                robots.Add(newRobot);
                myRobot.Energy -= energyLoss;

                result.NewRobotPosition = position;
                result.TotalEnergyChange = -energyLossToCreateNewRobot;

                Description = String.Format("New: {0}", result.NewRobotPosition);
            }
            else
            {
                Description = String.Format("FAILED: not enough energy to create new robot");
            }


            return result;
        }
    }

    
}
