using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars.GameObjects
{
    public class Crumbs : GameObject
    {
        private int energy;
        public int Energy
        {
            get
            {
                return energy;
            }

            set
            {
                energy = value;

                if (value <= 0)
                {
                    energy = 0;
                    DeleteMeLater = true;
                }
                else
                {
                    DeleteMeLater = false;
                }
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
