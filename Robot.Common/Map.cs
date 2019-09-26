using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robot.Common
{

    public enum MapSize
    {
        Small, Large
    }

    public enum MapResourceFill
    {
        Poor, Rich
    }

    public sealed class Map
    {
        public bool IsValid(Position position)
        {
            return (position.X >= 0) && (position.X < MaxPozition.X) && (position.Y >= 0) &&
                   (position.Y < MaxPozition.Y);
        }

        public Position FindFreeCell(Position nearPosition, IList<Robot> robots)
        {
            var distance = 1;
            while (distance < 100)
            {
                for (int i = -distance; i <= distance; i++)
                    for (int j = -distance; j <= distance; j++)
                    {
                        var newPos = new Position(nearPosition.X + i, nearPosition.Y + j);
                        if (!IsValid(newPos))
                            continue;


                        if (!robots.Any(r => r.Position == newPos))
                            return newPos;

                    }
                distance++;
            }
            throw new Exception("All cells are filled");

        }


        private readonly Position _maxPozitionForSmallMap = new Position() { X = 100, Y = 100 };

        public Position MinPozition = new Position() { X = 0, Y = 0 };
        public Position MaxPozition;

        public IList<EnergyStation> Stations = new List<EnergyStation>();

        public EnergyStation GetResource(Position pozition)
        {
            return Stations.FirstOrDefault(x => x.Position.X == pozition.X && x.Position.Y == pozition.Y);
        }

        /// <summary>
        /// Return all station in the square  
        /// </summary>
        /// <param name="pozition">square center</param>
        /// <param name="distance"> Max distance from center</param>
        /// <returns></returns>
        public List<EnergyStation> GetNearbyResources(Position pozition, int distance)
        {
            return Stations.Where(station => (Math.Abs(station.Position.X - pozition.X) <= distance)
                && (Math.Abs(station.Position.Y - pozition.Y) <= distance)).ToList();
        }

        private List<EnergyStation> CopyResources()
        {
            return Stations.Select(energyResource => new EnergyStation() { Energy = energyResource.Energy, Position = energyResource.Position.Copy(), RecoveryRate = energyResource.RecoveryRate }).ToList();
        }

        public Map Copy()
        {
            return new Map() { MaxPozition = _maxPozitionForSmallMap, Stations = CopyResources() };
        }


        public Map()
        {

        }

        public Map(int variant, int initialRobotCount)
        {


            MaxPozition = _maxPozitionForSmallMap;

            Variant.Initialize(variant);
            var mapParams = Variant.GetInstance();

            var numberOfResources = (int)(initialRobotCount * mapParams.EnergyStationForAttendant);

            var listOfUsed = new List<Position>();
            Random r = new Random();
            Position newPosition = null;



            for (int i = 0; i < numberOfResources; i++)
            {

                do
                {

                    newPosition = new Position() { X = r.Next(MaxPozition.X), Y = r.Next(MaxPozition.Y) };

                } while (listOfUsed.Contains(newPosition));
                //So station could not be at the same place
                listOfUsed.Add(newPosition);
                var minGrowth = mapParams.MinEnergyGrowth;
                var maxGrowth = mapParams.MaxEnergyGrowth;


                Stations.Add(new EnergyStation() { RecoveryRate = minGrowth + r.Next(maxGrowth - minGrowth), Energy = 0, Position = newPosition });
            }

        }
    }
}
