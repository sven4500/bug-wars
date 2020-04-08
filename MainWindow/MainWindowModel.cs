using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel; // ObservableCollection<>
using BugWars.GameObjects; // Bug, Egg, Food, ..

namespace BugWars
{
    public class MainWindowModel
    {
        readonly private Config conf;

        private ObservableCollection<Bug> bugs = new ObservableCollection<Bug>();
        public ObservableCollection<Bug> Bugs { get { return bugs; } }

        public MainWindowModel(Config _conf)
        {
            conf = _conf;

            Bug bug = new Bug();
            bug.PosX = 0;
            bug.PosY = 0;
            bug.Team = Bug.TeamEnum.Blue;
            bug.Sex = Bug.SexEnum.Male;

            Bugs.Add(bug);
        }

        public void Update()
        {
            Bugs[0].PosX++;
            Bugs[0].PosY++;
        }
    }
}
