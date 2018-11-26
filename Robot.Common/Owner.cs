using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robot.Common
{
    public sealed class Owner
    {

        public string Name { get; set; }

/*        private IRobotAlgorithm _algorithm;

        internal IRobotAlgorithm Algorithm { get { return _algorithm; } 
             set { _algorithm = value;
                _name = Algorithm.Author;
            }
        }*/

        public Owner Copy()
        {
            {
                return new Owner() {Name = this.Name};
            }
        }

    }
}
