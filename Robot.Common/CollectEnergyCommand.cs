using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robot.Common
{
    public sealed class CollectEnergyCommand : RobotCommand
    {
        public override UpdateViewAfterRobotStepEventArgs ChangeModel(IList<Robot> robots, int currentIndex, Map map)
        {
            var result = new UpdateViewAfterRobotStepEventArgs();

            var myRobot = robots[currentIndex];
            var resources = map.GetNearbyResources(myRobot.Position, Variant.GetInstance().CollectingDistance);
            if (resources.Count == 0)
                Description = "FAILED: no resource to collect energy";

            int totalCollectedEnergy = 0;

            foreach (var energyStation in resources)
            {
                if (energyStation != null)
                {
                    var energy = Math.Min(energyStation.Energy, Variant.GetInstance().MaxEnergyCanCollect);
                    myRobot.Energy += energy;
                    energyStation.Energy -= energy;

                    totalCollectedEnergy += energy;
                }
                else
                {
                    //Description = "ERROR: station is null";
                }
            }
            result.TotalEnergyChange = totalCollectedEnergy;
            Description = String.Format("Collect {0} total {1}", totalCollectedEnergy, myRobot.Energy);

            return result;
        }
    }
}