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
        public Dictionary<string, IRobotAlgorithm> Algorithms = new Dictionary<string, IRobotAlgorithm>();
        private int _roundNumber;
        private RobotStepCompletedEventHandler _callback;

        public Runner(Map map, RobotStepCompletedEventHandler callback)
        {
            Map = map;
            _callback = callback;
            InitializeOwnersAndRobots(50);
        }


        private OwnerStatistics CalculateOwnerStatistics(string ownerName)
        {
            var ownersStatistics = new OwnerStatistics();
            foreach (var robot in Robots)
            {
                if (robot.OwnerName != ownerName) continue;

                ownersStatistics.RobotsCount++;
                ownersStatistics.TotalEnergy += robot.Energy;
            }
            return ownersStatistics;
        }

        public List<OwnerStatistics> CalculateOwnerStatistics()
        {
            var ownersStatistics = new Dictionary<string, OwnerStatistics>();
            foreach (var robot in Robots)
            {
                if (!ownersStatistics.ContainsKey(robot.OwnerName))
                {

                    ownersStatistics.Add(robot.OwnerName, CalculateOwnerStatistics(robot.OwnerName));
                }
            }

            var result = new List<OwnerStatistics>();

            foreach (var owner in ownersStatistics.Keys)
            {
                var st = ownersStatistics[owner];
                st.Owner = owner;
                result.Add(st);
            }
            result.Sort((a, b) => string.Compare(a.Owner, b.Owner, StringComparison.Ordinal));
            return result;
        }


        public IList<Common.Robot> CopyRopots()
        {
            var ownersCopy = new Dictionary<Owner, Owner>();

            var copy = new List<Common.Robot>();
            foreach (var robot in Robots)
            {
                
                copy.Add(new Common.Robot { Energy = robot.Energy, Position = robot.Position.Copy(), OwnerName = robot.OwnerName});
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

            foreach (var algorithnm in list)
            {
                if (Algorithms.ContainsKey(algorithnm.Author))
                    throw new Exception($"At least 2 libraries of ssame author: {algorithnm.Author}. Tournament terminated.");
                Algorithms.Add(algorithnm.Author, algorithnm);
            }

            Robots = new List<Common.Robot>();

            var initialOwnersList = list.Select(algorithm => new Owner(){Name = algorithm.Author}).ToList();
            

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
                            OwnerName = Owners[ownerIndex].Name,
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
            args.OwnerName = Robots[_currentRobotIndex].OwnerName;
            var ownerName = args.OwnerName;
            args.RobotPosition = Robots[_currentRobotIndex].Position;

            if (Robots[_currentRobotIndex].Energy > 0)
            {
                try
                {
                    //make a copy to pass to the algorithm
                    var command = Algorithms[args.OwnerName].DoStep(CopyRopots(), _currentRobotIndex, Map.Copy());
                    //To make sure that student are not cheating by adding new class

                    var type = command.GetType();
                    var allowedCommands = new List<Type>() { typeof(Robot.Common.CreateNewRobotCommand), typeof(Robot.Common.MoveCommand), typeof(Robot.Common.CollectEnergyCommand) };

                    if (allowedCommands.Contains(type))
                    {
                        args = command.ChangeModel(Robots, _currentRobotIndex, Map);
                        args.OwnerName = ownerName;
                        args.RobotPosition = Robots[_currentRobotIndex].Position;
                        Logger.LogMessage(ownerName, command.Description, LogValue.Normal);
                    }
                    else
                    {
                        Logger.LogMessage(ownerName,
                            $"{Robots[_currentRobotIndex].OwnerName} is nasty cheater, let's kill his robot for that ))", LogValue.High);
                        Robots[_currentRobotIndex].Energy = 0;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogMessage(ownerName, $"Error: {e.Message} ", LogValue.Error);
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
        public string Owner;
        public int TotalEnergy { get; set; }
        public int RobotsCount { get; set; }
    }
}

