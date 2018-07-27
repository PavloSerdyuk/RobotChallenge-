using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Common;

namespace Robot.Common.Test
{
    [TestClass]
    public class MoveCommandTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var robots = new List<Robot>
                             {
                                 new Robot() {Energy = 1000, Position = new Position(0, 0)},
                                 new Robot() {Energy = 1000, Position = new Position(1, 1)}
                             };

            var c = new MoveCommand() {NewPosition = new Position(1, 1)};
            var map = new Map(){MaxPozition =  new Position(100,100), MinPozition = new Position(0,0), Stations = new List<EnergyStation>()};
            
            c.Apply(robots, 0, map );

            Assert.AreEqual(898, robots[0].Energy);
            Assert.AreEqual(robots[1].Energy, 1000);
        }
    }
}
