using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robot.Common
{
    public sealed class Owner
    {
        public string _name;
        public string Name {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        private IRobotAlgorithm _algorithm;
        // return Algorithm.Author; 
        public IRobotAlgorithm Algorithm { get { return _algorithm; } 
            set { _algorithm = value;
                _name = Algorithm.Author;
            }
        }

        public Owner Copy()
        {
            { return new Owner(){Name = this.Name}; }
        }
        
    }
}
