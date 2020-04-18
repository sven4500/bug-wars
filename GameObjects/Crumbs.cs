using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars.GameObjects
{
    public class Crumbs : GameObject
    {
        int energy;

        int Energy
        {
            get
            {
                return energy;
            }

            set
            {
                energy = value;
                if (value <= 0)
                    DeleteMeLater = true;
            }
        }

        public Crumbs(int x, int y, int energy)
        {
            PosX = x;
            PosY = y;
            Energy = energy;
        }
    }
}
