using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robot.Common
{
    public sealed class CreateNewRobotCommand : RobotCommand
    {

        public int NewRobotEnergy { get; set; } = 100;

        public override UpdateViewAfterRobotStepEventArgs ChangeModel(IList<Robot> robots, int currentIndex, Map map)
        {
            var result = new UpdateViewAfterRobotStepEventArgs();
            var myRobot = robots[currentIndex];
            var energyLossToCreateNewRobot = Variant.GetInstance().EnergyLossToCreateNewRobot;
            var energyLoss = energyLossToCreateNewRobot + NewRobotEnergy;

            //Fix hack with incorrect value of energy
            if (NewRobotEnergy <= 0 || NewRobotEnergy > Int32.MaxValue / 2)
            {
                Description = $"FAILED: illegal value for new robot energy of {myRobot.OwnerName} .";

            }
            else if (robots.Count(r => r.OwnerName == myRobot.OwnerName) >= 100)
            {
                Description = $"FAILED: number of {myRobot.OwnerName} robots reached 100.";
            }
            else if (myRobot.Energy > energyLoss)
            {
                var position = map.FindFreeCell(myRobot.Position, robots);
                var newRobot = new Robot() { Position = position, Energy = NewRobotEnergy, OwnerName = myRobot.OwnerName };
                robots.Add(newRobot);
                myRobot.Energy -= energyLoss;

                result.NewRobotPosition = position;
                result.TotalEnergyChange = -energyLossToCreateNewRobot;

                Description = $"New: {result.NewRobotPosition} remains energy {myRobot.Energy}";
            }
            else
            {
                Description = "FAILED: not enough energy to create new robot";
            }

            return result;
        }
    }


}