using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robot.Common
{
    public sealed class MoveCommand : RobotCommand
    {
        public Position NewPosition { get; set; }

        private int Min2D(int x1, int x2)
        {
            int[] nums = 
            {
                (int) Math.Pow(x1 - x2, 2),
                (int) Math.Pow(x1 - x2 + 100, 2),
                (int) Math.Pow(x1 - x2 - 100, 2)
            };
            return nums.Min();
        }

        private int CalculateLoss(Position p1, Position p2)
        {
            return Min2D(p1.X , p2.X) + Min2D(p1.Y , p2.Y);
        }

        public override UpdateViewAfterRobotStepEventArgs ChangeModel(IList<Robot> robots, int currentIndex, Map map)
        {
            var result = new UpdateViewAfterRobotStepEventArgs();

            //skip movement if not valid
            if (!map.IsValid(NewPosition))
            {
                Description = $"FAILED: {NewPosition} position not valid ";
                return result;
            }


            var myRobot = robots[currentIndex];
            var oldPosition = robots[currentIndex].Position;
            var moveEnergyLoss = CalculateLoss(NewPosition, oldPosition);

            Robot movedRobot = null;

            foreach (var robot in robots)
            {

                if ((robot != myRobot) && (robot.Position == NewPosition))
                {
                    //moving 
                    //var energyOfFightLoss =  Math.Min(robot.Energy, myRobot.Energy);
                    moveEnergyLoss += Variant.GetInstance().AttackEnergyLoss;
                    movedRobot = robot;
                }
            }


            //if not enough energy than skip the movement
            if (myRobot.Energy < moveEnergyLoss)
            {
                Description = "FAILED: not enough energy to move";
                return new UpdateViewAfterRobotStepEventArgs();
            }


            result.TotalEnergyChange = -moveEnergyLoss;
            result.MovedFrom = new List<Position>() { oldPosition };
            result.MovedTo = new List<Position>() { NewPosition };

            myRobot.Energy -= moveEnergyLoss;
            myRobot.Position = NewPosition;
            Description = String.Format("MOVE: {0}-> {1}", oldPosition, NewPosition);

            if (movedRobot != null)
            {
                var newX = 2*NewPosition.X - oldPosition.X;
                var newY = 2 * NewPosition.Y - oldPosition.Y;
                var movedFrom = movedRobot.Position;
                var movedTo = map.FindFreeCell(new Position(newX, newY), robots); ;
                movedRobot.Position = movedTo;

                //Stole energy
                var stoleRate = Variant.GetInstance().StoleRateEnergyAtAttack;
                myRobot.Energy += (int)(movedRobot.Energy*stoleRate);
                movedRobot.Energy -= (int)(movedRobot.Energy * stoleRate);
                result.MovedFrom.Insert(0, movedFrom);
                result.MovedTo.Insert(0, movedRobot.Position);
                Description = $"Attacked {movedRobot.OwnerName} robot at ({NewPosition})";
            }

            return result;
        }
    }
}
