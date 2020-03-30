using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars
{
    public class Config
    {
        /*public Config(uint _mapWidth, uint _mapHeight, uint _feedRate,
            uint _bugCountBlue, uint _bugCountRed, uint _bugHealthBlue, uint _bugHealthRed,
            uint _bugSpeedBlue, uint _bugSpeedRed, uint _bugEatRateBlue, uint _bugEatRateRed,
            uint _bugStrengthBlue, uint _bugStrengthRed)
        {
            mapWidth = _mapWidth;
            mapHeight = _mapHeight;

            feedRate = _feedRate;

            bugCountBlue = _bugCountBlue;
            bugCountRed = _bugCountRed;

            bugHealthBlue = _bugHealthBlue;
            bugHealthRed = _bugHealthRed;

            bugSpeedBlue = _bugSpeedBlue;
            bugSpeedRed = _bugSpeedRed;

            bugEatRateBlue = _bugEatRateBlue;
            bugEatRateRed = _bugEatRateRed;

            bugStrengthBlue = _bugStrengthBlue;
            bugStrengthRed = _bugStrengthRed;
        }*/

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

        public uint BugEatRateBlue { get; set; }
        public uint BugEatRateRed { get; set; }

        public uint BugStrengthBlue { get; set; }
        public uint BugStrengthRed { get; set; }
    }
}
