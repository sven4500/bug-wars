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

        readonly private object bugLock = new object();

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

        private IGameObject Eat(Task<IGameObject> antecedent)
        {
            return null;
        }

        private IGameObject Pair(Task<IGameObject> before)
        {
            return null;
        }

        private IGameObject Fight(Task<IGameObject> antecedent)
        {
            return null;
        }

        private IGameObject Move(Task<IGameObject> antecedent)
        {
            Bug bug = antecedent.Result as Bug;
            if (bug == null) { return null; }
            
            lock (bugLock)
            {

            }

            return bug;
        }

        /*private Task<T> AttachPipe<T>(Task<T> antecedent, Func<Task<T>, T> func) where T : class
        {
            return antecedent.ContinueWith<T>(func);
        }*/

        public void Update()
        {
            List<Task> tasks = new List<Task>();

            foreach (var bug in bugsBlue)
            {
                Task task = Task
                    .Run(() => { return bug as IGameObject; })
                    .ContinueWith<IGameObject>(Move)
                    /*.ContinueWith<IGameObject>(Move)*/;
                tasks.Add(task);
            }

            foreach (var bug in bugsRed)
            {
                Task task = Task
                    .Run(() => { return bug as IGameObject; })
                    .ContinueWith<IGameObject>(Move)
                    /*.ContinueWith<IGameObject>(Move)*/;
                tasks.Add(task);
            }

            // Task.WaitAll()
            foreach(var task in tasks)
                task.Wait();
        }

        // Метод генерирующий список уникальных значений. Необходим для того
        // чтобы раскидать жуков по произвольным клеткам поля.
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
                bug.Sex = Bug.GetRandomSex();
                bug.Direction = Bug.GetRandomDirection();

                bugs.Add(bug);
            }

            return bugs;
        }
    }
}
