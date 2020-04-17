using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars.GameObjects
{
    public class Egg : GameObject
    {
        public Bug.TeamEnum Team { get; set; }

        private int hatchCounter;
        public int HatchCounter
        {
            get { return hatchCounter; }
            set { hatchCounter = value > 0 ? value : 0; }
        }

        public Egg(int x, int y, Bug.TeamEnum team, int hatchCounter)
        {
            PosX = x;
            PosY = y;
            Team = team;
            HatchCounter = hatchCounter;
        }
    }
}
