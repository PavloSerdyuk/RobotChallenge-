using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Robot.Common;



namespace Robot.Tournament
{
    public class RobotStepHistory
    {
        public bool IsAlive { get; set; }
        public Position OldPozition { get; set; }
        public Position NewPozition { get; set; }
    }

    public class RobotStepCompletedArgs : EventArgs
    {
        public Common.Robot Robot;
        public RobotStepHistory History;
    }

    

    public class Runner
    {
        public const int InitialRobotsCount = 10;


        public Map Map { get; set; }
        public IList<Common.Robot> Robots { get; set; }
        public List<Owner> Owners;
        private int _roundNumber;
        private RobotStepCompletedEventHandler _callback;

        public Runner(Map map, RobotStepCompletedEventHandler callback)
        {
            Map = map;
            _callback = callback;
            InitializeOwnersAndRobots(50);
        }

        /*
        private void LogRound(Dictionary<Owner, OwnerStatistics> ownerStatistics)
        {
            Log(String.Format("Round # {0}", _roundNumber), LogValue.High);
            foreach (var owner in ownerStatistics.Keys)
            {
                var stat = ownerStatistics[owner];
                if (stat.RobotsCount == 0)
                    Log(String.Format("{0} lose the channange: place # {1}", owner.Name, ownerStatistics.Count), LogValue.High);
                else
                {
                    Log(String.Format("{0} lose the channange: place # {1}", owner.Name, ownerStatistics.Count), LogValue.Medium);
                    
                }

            }
        }*/


        private void PublishReslut(Dictionary<Owner, OwnerStatistics> ownerStatistics)
        {

        }

        private OwnerStatistics CalculateOwnerStatistics(Owner owner)
        {
            var ownersStatistics = new OwnerStatistics();
            foreach (var robot in Robots)
            {
                if (robot.Owner == owner)
                {
                    ownersStatistics.RobotsCount++;
                    ownersStatistics.TotalEnergy += robot.Energy;
                }
            }
            return ownersStatistics;
        }

        public List<OwnerStatistics> CalculateOwnerStatistics()
        {
            var ownersStatistics = new Dictionary<Owner, OwnerStatistics>();
            foreach (var robot in Robots)
            {
                if (!ownersStatistics.ContainsKey(robot.Owner))
                {

                    ownersStatistics.Add(robot.Owner, CalculateOwnerStatistics(robot.Owner));
                }
            }

            var result = new List<OwnerStatistics>();
            foreach (var owner in ownersStatistics.Keys)
            {
                var st = ownersStatistics[owner];
                st.Owner = owner;
                result.Add(st);
            }
            result.Sort((a, b) => string.Compare(a.Owner.Name, b.Owner.Name, StringComparison.Ordinal));
            return result;
        }


        public IList<Common.Robot> CopyRopots()
        {
            var ownersCopy = new Dictionary<Owner, Owner>();

            var copy = new List<Common.Robot>();
            foreach (var robot in Robots)
            {
                
                if (! ownersCopy.ContainsKey(robot.Owner))
                {
                    ownersCopy.Add(robot.Owner, robot.Owner.Copy());
                }

                copy.Add(new Common.Robot { Energy = robot.Energy, Position = robot.Position.Copy(), Owner = ownersCopy[robot.Owner] });
            }

            return copy;
        }





        public void UpdateResources()
        {
            foreach (var energyResource in Map.Stations)
            {
                energyResource.Energy += energyResource.RecoveryRate;
                energyResource.Energy = Math.Min(energyResource.Energy, Variant.GetInstance().MaxStationEnergy );
            }
        }

        public int MaxNumbersOfRound { set; get; }

        public void InitializeOwnersAndRobots(int maxNumbersOfRound)
        {
            MaxNumbersOfRound = maxNumbersOfRound;
            _roundNumber = 0;

            //  RobotRegistry dsr = new RobotRegistry();
            // ContainerBootstrapper.BootstrapStructureMap();
            //var t = ObjectFactory.GetInstance<IRobotAlgorithm>();
            //var list = ObjectFactory.GetAllInstances<IRobotAlgorithm>().ToList();
            var list = ReflectionScanner.Scan();
            Robots = new List<Common.Robot>();

            var initialOwnersList = list.Select(algorithm => new Owner {Algorithm = algorithm}).ToList();
            var r = new Random();

            //Make order random
            Owners = new List<Owner>();

            while (initialOwnersList.Count > 0)
            {
                var index = r.Next(initialOwnersList.Count);
                Owners.Add(initialOwnersList[index]);
                initialOwnersList.RemoveAt(index);
            }

            var listPosition = new List<Position>();

            while (listPosition.Count < Owners.Count)
            {
                var newPosition = new Position {X = r.Next(Map.MaxPozition.X), Y = r.Next(Map.MaxPozition.Y)};

                var nearPozition = listPosition.FirstOrDefault(pos =>
                    (Math.Abs(newPosition.X - pos.X) < 10) && (Math.Abs(newPosition.Y - pos.Y) < 10));
                if (nearPozition == null)
                    listPosition.Add(newPosition);
            }


            for (int i = 0; i < InitialRobotsCount; i++)
            {
                for (int ownerIndex = 0; ownerIndex < Owners.Count; ownerIndex++)
                {
                    Position position;
                    do
                    {
                        position = new Position
                        {
                            X = listPosition[ownerIndex].X - 20 + r.Next(41),
                            Y = listPosition[ownerIndex].Y - 20 + r.Next(41),
                        };
                    } while (!Map.IsValid(position) || (Robots.Any(rob => rob.Position == position)));

                    Robots.Add(new Common.Robot
                        {
                            Energy = 100,
                            Owner = Owners[ownerIndex],
                            Position = position
                        }
                    );
                }
            }


        }

        private int _currentRobotIndex;

        public void DoStep()
        {
            var args = new UpdateViewAfterRobotStepEventArgs();
            if (_roundNumber > MaxNumbersOfRound)
                return;

            if (_currentRobotIndex >= Robots.Count)
                PrepareNextRound();
            var owner = Robots[_currentRobotIndex].Owner;
            args.Owner = owner;
            args.RobotPosition = Robots[_currentRobotIndex].Position;

            if (Robots[_currentRobotIndex].Energy > 0)
            {
                try
                {
                    //make a copy to pass to the algorithm
                    var command = owner.Algorithm.DoStep(CopyRopots(), _currentRobotIndex, Map.Copy());
                    //To make sure that student are not cheating by adding new class

                    var type = command.GetType();
                    var allowedCommands = new List<Type>() { typeof(Robot.Common.CreateNewRobotCommand), typeof(Robot.Common.MoveCommand), typeof(Robot.Common.CollectEnergyCommand) };

                    if (allowedCommands.Contains(type))
                    {
                        //command.RobotStepCompleted += _callback;
                        args = command.ChangeModel(Robots, _currentRobotIndex, Map);
                        args.Owner = owner;
                        args.RobotPosition = Robots[_currentRobotIndex].Position;
                        //command.Apply(Robots, _currentRobotIndex, Map);
                        Logger.LogMessage(owner, command.Description, LogValue.Normal);
                    }
                    else
                    {
                        Logger.LogMessage(owner,
                            $"{Robots[_currentRobotIndex].Owner.Name} is nasty cheater, let's kill his robot for that ))", LogValue.High);
                        Robots[_currentRobotIndex].Energy = 0;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogMessage(owner, $"Error: {e.Message} ", LogValue.Error);
                    //simply do nothing
                    _callback(null, args);

                }
            }

            _currentRobotIndex++;
            _callback(null, args);
        }

        public void PrepareNextRound()
        {
            _currentRobotIndex = 0;
            _roundNumber++;
            Logger.LogRound(_roundNumber);
            UpdateResources();
        }
    }
    


    public class OwnerStatistics
    {
        public Owner Owner;
        public int TotalEnergy { get; set; }
        public int RobotsCount { get; set; }
    }
}

