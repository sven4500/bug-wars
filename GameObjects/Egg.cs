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
            get
            {
                return hatchCounter;
            }

            set
            {
                hatchCounter = value;

                if (hatchCounter <= 0)
                {
                    hatchCounter = 0;
                    DeleteMeLater = true;
                }
                else
                {
                    DeleteMeLater = false;
                }
            }
        }

        public bool IsHatching
        {
            get
            {
                return hatchCounter <= 0;
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
