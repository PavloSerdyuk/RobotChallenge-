using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robot.Common
{
    
    public interface IRobotAlgorithm
    {
        string Author { get; }
        string Description { get; }

        /// <summary>
        /// Methods to oveeride in your algorithm
        /// </summary>
        /// <param name="robots"></param>
        /// <param name="robotToMoveIndex">index of the robot in the list to move</param>
        /// <param name="map"></param>
        /// <returns></returns>
        RobotCommand DoStep(IList<Robot> robots, int robotToMoveIndex, Map map);
    }
}
