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
        public uint CanvasWidth { get; set; }
        public uint CanvasHeight { get; set; }

        public uint MapWidth { get; set; }
        public uint MapHeight { get; set; }

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
        protected GridLine()
        { }
    }
}
