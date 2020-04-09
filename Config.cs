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

        public uint BugCountTotal
        {
            get { return BugCountRed + BugCountBlue; }
        }

        public uint MapSize
        {
            get { return MapWidth * MapHeight; }
        }

        public uint MapWidth { get; set; }
        public uint MapHeight { get; set; }

        public uint FeedRate { get; set; }

        public uint BugCountBlue { get; set; }
        public uint BugCountRed { get; set; }

        public uint BugHealthBlue { get; set; }
        public uint BugHealthRed { get; set; }

        public uint BugSpeedBlue { get; set; }
        public uint BugSpeedRed { get; set; }

        public uint BugAppetiteBlue { get; set; }
        public uint BugAppetiteRed { get; set; }

        public uint BugStrengthBlue { get; set; }
        public uint BugStrengthRed { get; set; }
    }
}
