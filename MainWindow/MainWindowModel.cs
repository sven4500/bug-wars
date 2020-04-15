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

        // Глобальный рандомизатор модели. Используется при расстановке жуков
        // по игровому полю в произвольном порядке.
        readonly private Random random = new Random(Environment.TickCount);

        // Так как коллекции жуков две (синяя комманда и красная комманда), то
        // создаём один общий элемент синхронизации.
        readonly private object bugLock = new object();

        readonly private ObservableCollection<Bug> bugsBlue = new ObservableCollection<Bug>();
        public ObservableCollection<Bug> BugsBlue { get { return bugsBlue; } }

        readonly private ObservableCollection<Bug> bugsRed = new ObservableCollection<Bug>();
        public ObservableCollection<Bug> BugsRed { get { return bugsRed; } }

        public MainWindowModel(Config _conf)
        {
            conf = _conf;

            {
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

            int desiredX = bug.PosX;
            int desiredY = bug.PosY;

            if (bug.Direction == Bug.DirectionEnum.Up)
                --desiredY;
            else if (bug.Direction == Bug.DirectionEnum.Down)
                ++desiredY;
            else if (bug.Direction == Bug.DirectionEnum.Lef)
                --desiredX;
            else if (bug.Direction == Bug.DirectionEnum.Right)
                ++desiredX;

            // Находимся на границе игрового поля. Дальше идти не можем поэтому
            // выбираем новое случайное направление.
            if (desiredX < 0 || desiredX >= conf.MapWidth || desiredY < 0 || desiredY >= conf.MapHeight)
            {
                bug.Direction = Bug.GetRandomDirection();
                return bug;
            }

            lock (bugLock)
            {
                IEnumerable<Bug> blueBugs =
                    from temp in bugsBlue
                    where temp.PosX == desiredX && temp.PosY == desiredY
                    select temp;

                IEnumerable<Bug> redBugs =
                    from temp in bugsRed
                    where temp.PosX == desiredX && temp.PosY == desiredY
                    select temp;

                // В обоих командах смотрим есть ли такой жук который находится
                // на желаемом поле. Если да, то меняем направление.
                if (blueBugs.Any() == false && redBugs.Any() == false)
                {
                    bug.PosX = desiredX;
                    bug.PosY = desiredY;
                }
                else
                {
                    bug.Direction = Bug.GetRandomDirection();
                }
            }

            // Здесь направление жука меняется произвольным образом.
            if (random.Next() % 100 > 60 == true)
            {
                bug.Direction = Bug.GetRandomDirection();
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
                bug.PosX = posX.Current;
                bug.PosY = posY.Current;
                bug.Team = team;
                bug.Sex = Bug.GetRandomSex();
                bug.Direction = Bug.GetRandomDirection();

                bugs.Add(bug);
            }

            return bugs;
        }
    }
}
