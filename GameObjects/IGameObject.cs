using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars.GameObjects
{
    // Интерфейс игрового объекта предсталяет собой точку на плоскости игровой
    // карты. GameObject один из нескольких классов реализует этот интерфейс и
    // также задаёт ограничение по пространству MapWidth и MapHeight.
    public interface IGameObject
    {
        int PosX { get; set; }
        int PosY { get; set; }

        bool DeleteMeLater { get; set; }
    }
}
