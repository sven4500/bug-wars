using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars
{
    public class HorizontalGridLine : GridLine
    {
        public HorizontalGridLine(uint mapWidth, uint mapHeight, uint canvasWidth, uint canvasHeight)
            : base(mapWidth, mapHeight, canvasWidth, canvasHeight)
        { }
    }
}
