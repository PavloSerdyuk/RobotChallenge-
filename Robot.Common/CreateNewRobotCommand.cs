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
            if (NewRobotEnergy <= 0 || NewRobotEnergy > Int32.MaxValue/2)
            {
                Description = $"FAILED: illegal value for new robot energy of {myRobot.Owner.Name} .";

            } else if (robots.Count(r => r.Owner.Name == myRobot.Owner.Name) >= 100)
            {
                Description = $"FAILED: number of {myRobot.Owner.Name} robots reached 100.";
            }
            else if (myRobot.Energy > energyLoss)
            {
                var position = map.FindFreeCell(myRobot.Position, robots);
                var newRobot = new Robot() { Position = position, Energy = NewRobotEnergy, Owner = myRobot.Owner };
                robots.Add(newRobot);
                myRobot.Energy -= energyLoss;

                result.NewRobotPosition = position;
                result.TotalEnergyChange = -energyLossToCreateNewRobot;

                Description = $"New: {result.NewRobotPosition}";
            }
            else
            {
                Description = "FAILED: not enough energy to create new robot";
            }

            return result;
        }
    }

    
}
