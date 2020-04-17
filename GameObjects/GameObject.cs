using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars.GameObjects
{
    public class GameObject : IGameObject
    {
        public int PosX { get; set; }
        public int PosY { get; set; }

        public bool DeleteMeLater { get; set; }

        protected GameObject()
        {
            PosX = 0;
            PosY = 0;

            DeleteMeLater = false;
        }
    }
}
