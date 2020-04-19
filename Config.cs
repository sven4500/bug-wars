using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars
{
    public class Config
    {
        public bool IsValid
        {
            get { return (MapSize >= BugCountTotal) && (BugHealthBlue > 0) && (BugHealthRed > 0); }
        }

        public int BugCountTotal
        {
            get { return BugCountRed + BugCountBlue; }
        }

        public int MapSize
        {
            get { return MapWidth * MapHeight; }
        }

        public int ChangeDirectionChance { get; set; }

        public int ToxicHit { get; set; }

        public int MapWidth { get; set; }
        public int MapHeight { get; set; }

        public int FeedRate { get; set; }
        public int CrumbEnergy { get; set; }

        public int BugCountBlue { get; set; }
        public int BugCountRed { get; set; }

        public int BugHealthBlue { get; set; }
        public int BugHealthRed { get; set; }

        public int BugAppetiteBlue { get; set; }
        public int BugAppetiteRed { get; set; }

        public int BugStrengthBlue { get; set; }
        public int BugStrengthRed { get; set; }
    }
}
