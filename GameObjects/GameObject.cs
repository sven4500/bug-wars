using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars.GameObjects
{
    public class GameObject : IGameObject
    {
        public uint MapWidth { get; private set; }
        public uint MapHeight { get; private set; }

        private uint posX;
        uint IGameObject.PosX { get { return posX; } set { posX = value % MapWidth; } }

        private uint posY;
        uint IGameObject.PosY { get { return posY; } set { posY = value % MapHeight; } }

        GameObject(uint _mapWidth, uint _mapHeight)
        {
            MapWidth = _mapWidth;
            MapHeight = _mapHeight;
        }
    }
}
