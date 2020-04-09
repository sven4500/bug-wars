using System;
using System.Diagnostics; // Debug.Assert
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

        readonly private ObservableCollection<Bug> bugsBlue = new ObservableCollection<Bug>();
        public ObservableCollection<Bug> BugsBlue { get { return bugsBlue; } }

        readonly private ObservableCollection<Bug> bugsRed = new ObservableCollection<Bug>();
        public ObservableCollection<Bug> BugsRed { get { return bugsRed; } }

        public MainWindowModel(Config _conf)
        {
            conf = _conf;

            var random = new Random(Environment.TickCount);
            bugsBlue = ReleaseBugs(Bug.TeamEnum.Blue, random);
            bugsRed = ReleaseBugs(Bug.TeamEnum.Red, random);
        }

        public void Update()
        {
            /*BugsBlue[0].PosX++;
            BugsBlue[0].PosY++;*/
        }

        // Метод генерирующий список уникальных значений.
        // https://stackoverflow.com/questions/14473321/generating-random-unique-values-c-sharp
        static private List<int> RandomUniqueList(int minValue, int maxValue, int capacity, Random random)
        {
            Debug.Assert(maxValue >= minValue);

            var list = new List<int>(capacity);
            int howMuch = Math.Min(Math.Abs(maxValue - minValue), capacity);

            for (int i = 0; i < howMuch; ++i)
            {
                int value = 0;

                do
                {
                    value = random.Next(minValue, maxValue);
                } while (list.Contains(value));

                list.Add(value);
            }

            return list;
        }

        private ObservableCollection<Bug> ReleaseBugs(Bug.TeamEnum team, Random random)
        {
            var bugs = new ObservableCollection<Bug>();

            // Кок получить произвольное значение из перечисления:
            // https://stackoverflow.com/questions/3132126/how-do-i-select-a-random-value-from-an-enumeration
            var sex = Enum.GetValues(typeof(Bug.SexEnum));

            var posX = RandomUniqueList(0, (int)conf.MapWidth, (int)conf.BugCountBlue, random);
            var posY = RandomUniqueList(0, (int)conf.MapHeight, (int)conf.BugCountBlue, random);

            for (int i = 0; i < conf.BugCountBlue; ++i)
            {
                Bug bug = new Bug();

                bug.PosX = (uint)posX[i];
                bug.PosY = (uint)posY[i];
                bug.Team = team;
                bug.Sex = (Bug.SexEnum)sex.GetValue(random.Next(sex.Length));

                bugs.Add(bug);
            }

            return bugs;
        }
    }
}
