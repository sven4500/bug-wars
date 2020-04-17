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

        // Элемент синхронизации запрещает изменение положения объектов.
        readonly private object globalLock = new object();

        readonly private ObservableCollection<Bug> bugsBlue = new ObservableCollection<Bug>();
        public ObservableCollection<Bug> BugsBlue { get { return bugsBlue; } }

        readonly private ObservableCollection<Bug> bugsRed = new ObservableCollection<Bug>();
        public ObservableCollection<Bug> BugsRed { get { return bugsRed; } }

        readonly private ObservableCollection<Egg> eggs = new ObservableCollection<Egg>();
        public ObservableCollection<Egg> Eggs { get { return eggs; } }

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

        private bool IsCellEmpty(int x, int y)
        {
            // Как захватить несколько объектов:
            // https://stackoverflow.com/questions/5975664/how-to-lock-several-objects
            lock (globalLock)
            {
                var enumerator1 =
                    from obj in BugsBlue
                    where obj.PosX == x && obj.PosY == y
                    select obj;

                var enumerator2 =
                    from obj in BugsRed
                    where obj.PosX == x && obj.PosY == y
                    select obj;

                var enumerator3 =
                    from obj in Eggs
                    where obj.PosX == x && obj.PosY == y
                    select obj;

                return !(enumerator1.Any() || enumerator2.Any() || enumerator3.Any());
            }
        }

        private IGameObject Fight(Task<IGameObject> antecedent)
        {
            Bug bug = antecedent.Result as Bug;

            if (bug == null)
            {
                return null;
            }

            lock (globalLock)
            {
                return bug;
            }
        }

        private IGameObject Eat(Task<IGameObject> antecedent)
        {
            Bug bug = antecedent.Result as Bug;

            if (bug == null)
            {
                return null;
            }

            lock (globalLock)
            {
                if (bug == null || bug.IsAtWar || bug.IsPairing)
                {
                    return bug;
                }

                return bug;
            }
        }

        private IGameObject Pair(Task<IGameObject> antecedent)
        {
            Bug bug = antecedent.Result as Bug;

            if (bug == null)
            {
                return null;
            }

            // Неважно как мы выходим из lock, объект будет освобождён.
            // https://stackoverflow.com/questions/9228114/c-sharp-lock-return-continue-break
            lock (globalLock)
            {
                if (bug.IsAtWar || bug.IsEating)
                {
                    return bug;
                }

                if (bug.IsPairing == false)
                {
                    if (bug.FertilityCounter == 0)
                    {
                        ObservableCollection<Bug> collection = ((bug.Team == Bug.TeamEnum.Blue) ? BugsBlue : BugsRed);

                        var otherBugs =
                            from otherBug in collection
                            where ((otherBug.PosX == bug.PosX + 1 && otherBug.PosY == bug.PosY)
                                || (otherBug.PosX == bug.PosX - 1 && otherBug.PosY == bug.PosY)
                                || (otherBug.PosX == bug.PosX && otherBug.PosY == bug.PosY + 1)
                                || (otherBug.PosX == bug.PosX && otherBug.PosY == bug.PosY - 1))
                                && otherBug.IsPairing == false
                                && otherBug.FertilityCounter == 0
                                && otherBug.Team == bug.Team
                                && otherBug.Sex != bug.Sex
                            select otherBug;

                        if (otherBugs.Any())
                        {
                            var otherBug = otherBugs.First();

                            Debug.Assert(bug.PairingCounter == 0);
                            Debug.Assert(bug.FertilityCounter == 0);

                            Debug.Assert(otherBug.PairingCounter == 0);
                            Debug.Assert(otherBug.FertilityCounter == 0);

                            // TODO: можно сделать конфигурируемым параметром.
                            otherBug.IsPairing = true;
                            otherBug.PairingCounter = 5;
                            otherBug.FertilityCounter = 3;

                            bug.IsPairing = true;
                            bug.PairingCounter = 5;
                            bug.FertilityCounter = 3;
                        }
                    }
                    else
                    {
                        Debug.Assert(bug.PairingCounter == 0);

                        bug.FertilityCounter--;
                    }
                }
                else
                {
                    bug.PairingCounter--;

                    if (bug.PairingCounter == 0)
                    {
                        bug.IsPairing = false;

                        if (bug.Sex == Bug.SexEnum.Female)
                        {
                            int[] desiredX = { bug.PosX + 1, bug.PosX - 1, bug.PosX, bug.PosY };
                            int[] desiredY = { bug.PosY, bug.PosY, bug.PosY + 1, bug.PosY - 1 };

                            for (int i = 0; i < 4; ++i)
                            {
                                if (IsCellEmpty(desiredX[i], desiredY[i]) == true)
                                {
                                    // TODO: ставим яйцо на свободную клетку
                                }
                            }
                        }
                    }
                }

                return bug;
            }
        }

        private IGameObject Move(Task<IGameObject> antecedent)
        {
            Bug bug = antecedent.Result as Bug;

            if (bug == null)
            {
                return null;
            }

            lock (globalLock)
            {
                if (bug.IsAtWar || bug.IsPairing || bug.IsEating)
                {
                    return bug;
                }

                int desiredX = bug.PosX;
                int desiredY = bug.PosY;

                if (bug.Direction == Bug.DirectionEnum.Up)
                {
                    --desiredY;
                }
                else if (bug.Direction == Bug.DirectionEnum.Down)
                {
                    ++desiredY;
                }
                else if (bug.Direction == Bug.DirectionEnum.Lef)
                {
                    --desiredX;
                }
                else if (bug.Direction == Bug.DirectionEnum.Right)
                {
                    ++desiredX;
                }

                // Находимся на границе игрового поля. Дальше идти не можем поэтому
                // выбираем новое случайное направление.
                if (desiredX < 0 || desiredX >= conf.MapWidth || desiredY < 0 || desiredY >= conf.MapHeight)
                {
                    bug.Direction = Bug.GetRandomDirection();
                    return bug;
                }

                // В обоих командах смотрим есть ли такой жук который находится
                // на желаемом поле. Если да, то меняем направление.
                if (IsCellEmpty(desiredX, desiredY))
                {
                    bug.PosX = desiredX;
                    bug.PosY = desiredY;

                    // Здесь направление жука меняется произвольным образом.
                    // TODO: можно сделать конфигурируемым параметром.
                    if (random.Next() % 100 > 60 == true)
                    {
                        bug.Direction = Bug.GetRandomDirection();
                    }
                }
                else
                {
                    bug.Direction = Bug.GetRandomDirection();
                }

                return bug;
            }
        }

        public void Update()
        {
            List<Task> tasks = new List<Task>();

            foreach (var bug in bugsBlue)
            {
                Task task = Task
                    .Run(() => { return bug as IGameObject; })
                    .ContinueWith<IGameObject>(Fight)
                    .ContinueWith<IGameObject>(Eat)
                    .ContinueWith<IGameObject>(Pair)
                    .ContinueWith<IGameObject>(Move);
                tasks.Add(task);
            }

            foreach (var bug in bugsRed)
            {
                Task task = Task
                    .Run(() => { return bug as IGameObject; })
                    .ContinueWith<IGameObject>(Fight)
                    .ContinueWith<IGameObject>(Eat)
                    .ContinueWith<IGameObject>(Pair)
                    .ContinueWith<IGameObject>(Move);
                tasks.Add(task);
            }

            // Task.WaitAll()
            foreach (var task in tasks)
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
