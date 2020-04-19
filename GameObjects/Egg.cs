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

        public int HatchCounter;

        public bool IsHatched
        {
            get
            {
                return HatchCounter <= 0;
            }
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
