using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars.GameObjects
{
    public class Bug : GameObject
    {
        public enum SexEnum { Male = 0, Female = 1 };
        public enum TeamEnum { Red = 0, Blue = 1 };

        public TeamEnum Team { get; set; }
        public SexEnum Sex { get; set; }
    }
}
