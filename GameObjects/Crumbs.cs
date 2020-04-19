using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars.GameObjects
{
    public class Crumbs : GameObject
    {
        public int Energy
        {
            get;
            private set;
        }

        public bool IsEmpty
        {
            get
            {
                return Energy <= 0;
            }
        }

        public int TakeEnergy(int energy)
        {
            energy = Math.Min(Energy, energy);
            Energy -= energy;
            return energy;
        }

        public Crumbs(int x, int y, int energy)
        {
            PosX = x;
            PosY = y;
            Energy = Math.Max(energy, 0);
        }
    }
}
