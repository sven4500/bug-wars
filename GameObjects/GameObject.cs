using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars.GameObjects
{
    public class GameObject : IGameObject
    {
        public uint PosX { get; set; }
        public uint PosY { get; set; }

        protected GameObject()
        { }
    }
}
