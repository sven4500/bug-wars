using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugWars.GameObjects; // IGameObject

namespace BugWars
{
    public class GridLine : IGameObject
    {
        public uint CanvasWidth { get; private set; }
        public uint CanvasHeight { get; private set; }

        public uint MapWidth { get; private set; }
        public uint MapHeight { get; private set; }

        public double StepWidth { get { return (double)CanvasWidth / MapWidth; } }
        public double StepHeight { get { return (double)CanvasHeight / MapHeight; } }

        // https://stackoverflow.com/questions/1253266/why-explicit-implementation-of-a-interface-can-not-be-public
        // Здесь хорошее замечание почему явная (explicit) реализация
        // интерфейсных методов не может быть публичной.
        //uint IGameObject.PosX { get; set; }
        //uint IGameObject.PosY { get; set; }

        public uint PosX { get; set; }
        public uint PosY { get; set; }

        // Защищённый метод чтобы нельзя было создать объект этого класса.
        protected GridLine(uint mapWidth, uint mapHeight, uint canvasWidth, uint canvasHeight)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;

            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;
        }
    }
}
