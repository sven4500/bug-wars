using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // ICommand

namespace BugWars
{
    public class ConfigWindowVM
    {
        public static readonly uint maxDim = 1000;

        // Здесь можно задать настройки по умолчанию.
        private Config conf = new Config
        {
            MapWidth = 40,
            MapHeight = 30,
            FeedRate = 0,
            BugCountBlue = 10,
            BugCountRed = 10,
            BugHealthBlue = 100,
            BugHealthRed = 100,
            BugSpeedBlue = 1,
            BugSpeedRed = 1,
            BugAppetiteBlue = 1,
            BugAppetiteRed = 1,
            BugStrengthBlue = 3,
            BugStrengthRed = 3
        };
        public Config Conf { get { return conf; } }

        public uint MapWidth { get { return Conf.MapWidth; } set { if (value <= maxDim) Conf.MapWidth = value; } }
        public uint MapHeight { get { return Conf.MapHeight; } set { if (value <= maxDim) Conf.MapHeight = value; } }

        readonly private ICommand playButtonCommand;
        public ICommand PlayButtonCommand
        {
            get { return playButtonCommand; }
        }

        public ConfigWindowVM()
        {
             playButtonCommand = new DelegateCommand(Function);
        }

        private void Function(object parameter)
        {
            if (Conf.IsValid)
            {
                MainWindow mainWindow = new MainWindow(Conf);
                mainWindow.ShowDialog();
            }
        }

    }
}
