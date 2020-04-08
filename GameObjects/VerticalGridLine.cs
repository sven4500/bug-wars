using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars
{
    public class VerticalGridLine : GridLine
    {
        public VerticalGridLine(uint mapWidth, uint mapHeight, uint canvasWidth, uint canvasHeight)
            : base(mapWidth, mapHeight, canvasWidth, canvasHeight)
        { }
    }
}
