using System;
using System.Threading;
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

            {
                var random = new Random(Environment.TickCount);

                var posX = RandomUniqueList(0, (int)conf.MapWidth, (int)conf.BugCountTotal, random);
                var teamBluePosX = posX.GetRange(0, (int)conf.BugCountBlue);
                var teamRedPosX = posX.GetRange((int)conf.BugCountBlue, (int)conf.BugCountRed);

                var posY = RandomUniqueList(0, (int)conf.MapHeight, (int)conf.BugCountTotal, random);
                var teamBluePosY = posY.GetRange(0, (int)conf.BugCountBlue);
                var teamRedPosY = posY.GetRange((int)conf.BugCountBlue, (int)conf.BugCountRed);

                bugsBlue = ReleaseBugs(teamBluePosX.GetEnumerator(), teamBluePosY.GetEnumerator(), Bug.TeamEnum.Blue, random);
                bugsRed = ReleaseBugs(teamRedPosX.GetEnumerator(), teamRedPosY.GetEnumerator(), Bug.TeamEnum.Red, random);
            }
        }

        public void Update()
        {

        }

        // Метод генерирующий список уникальных значений.
        // https://stackoverflow.com/questions/14473321/generating-random-unique-values-c-sharp
        static private List<int> RandomUniqueList(int minValue, int maxValue, int count, Random random)
        {
            Debug.Assert(maxValue >= minValue);

            count = Math.Min(Math.Abs(maxValue - minValue), count);
            var list = new List<int>(count);

            for (int i = 0; i < count; ++i)
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

        private ObservableCollection<Bug> ReleaseBugs(IEnumerator<int> posX, IEnumerator<int> posY, Bug.TeamEnum team, Random random)
        {
            var bugs = new ObservableCollection<Bug>();

            // Кок получить произвольное значение из перечисления:
            // https://stackoverflow.com/questions/3132126/how-do-i-select-a-random-value-from-an-enumeration
            var sex = Enum.GetValues(typeof(Bug.SexEnum));

            for (int i = 0; i < conf.BugCountBlue; ++i)
            {
                var advanceXSuccess = posX.MoveNext();
                var advanceYSuccess = posY.MoveNext();

                if (advanceXSuccess == false || advanceYSuccess == false)
                    throw new Exception();

                Bug bug = new Bug();

                bug.PosX = (uint)posX.Current;
                bug.PosY = (uint)posY.Current;
                bug.Team = team;
                bug.Sex = (Bug.SexEnum)sex.GetValue(random.Next(sex.Length));

                bugs.Add(bug);
            }

            return bugs;
        }
    }
}
