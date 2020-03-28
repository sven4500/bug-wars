using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // ICommand

namespace BugWars
{
    public class MainWindowVM
    {
        public static readonly uint maxDim = 1000;

        public uint BugCountTotal
        {
            get { return BugCountRed + BugCountBlue; }
        }

        public uint MapSize
        {
            get { return MapWidth * MapHeight; }
        }

        private uint mapWidth = 200;
        public uint MapWidth
        {
            get { return mapWidth; }
            set { mapWidth = (value < maxDim && value * MapHeight > BugCountTotal) ? value : maxDim; }
        }

        private uint mapHeight = 100;
        public uint MapHeight
        {
            get { return mapHeight; }
            set { mapHeight = (value < maxDim && value * MapHeight > BugCountTotal) ? value : maxDim; }
        }

        private uint feedRate = 0;
        public uint FeedRate
        {
            get { return feedRate; }
            set { feedRate = value; }
        }

        // Далее следует описание настроек обоих команд.
        private uint bugCountBlue = 10;
        public uint BugCountBlue
        {
            get { return bugCountBlue; }
            set { bugCountBlue = (value + BugCountRed < MapSize) ? value : bugCountBlue; }
        }

        private uint bugCountRed = 10;
        public uint BugCountRed
        {
            get { return bugCountRed; }
            set { bugCountRed = (value + BugCountBlue < MapSize) ? value : bugCountRed; }
        }

        private uint bugHealthBlue = 100;
        public uint BugHealthBlue
        {
            get { return bugHealthBlue; }
            set { bugHealthBlue = (value > 0) ? value : bugHealthBlue; }
        }

        private uint bugHealthRed = 100;
        public uint BugHealthRed
        {
            get { return bugHealthRed; }
            set { bugHealthRed = (value > 0) ? value : bugHealthRed; }
        }

        private uint bugSpeedBlue = 1;
        public uint BugSpeedBlue
        {
            get { return bugSpeedBlue; }
            set { bugSpeedBlue = value; }
        }

        private uint bugSpeedRed = 1;
        public uint BugSpeedRed
        {
            get { return bugSpeedRed; }
            set { bugSpeedRed = value; }
        }

        private uint bugEatRateBlue = 1;
        public uint BugEatRateBlue
        {
            get { return bugEatRateBlue; }
            set { bugEatRateBlue = value; }
        }

        private uint bugEatRateRed = 1;
        public uint BugEatRateRed
        {
            get { return bugEatRateRed; }
            set { bugEatRateRed = value; }
        }

        private uint bugStrengthBlue = 3;
        public uint BugStrengthBlue
        {
            get { return bugStrengthBlue; }
            set { bugStrengthBlue = value; }
        }

        private uint bugStrengthRed = 3;
        public uint BugStrengthRed
        {
            get { return bugStrengthRed; }
            set { bugStrengthRed = value; }
        }

        readonly private ICommand playButtonCommand;
        public ICommand PlayButtonCommand
        {
            get { return playButtonCommand; }
        }

        public MainWindowVM()
        {
             playButtonCommand = new DelegateCommand(Function);
        }

        private void Function(object parameter)
        {
            System.Windows.MessageBox.Show("Игра скоро начнётся");
        }

    }
}
