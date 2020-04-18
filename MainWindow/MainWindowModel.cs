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

        readonly private List<Bug> bugsBlue = new List<Bug>();
        public List<Bug> BugsBlue
        {
            get { return bugsBlue; }
        }

        readonly private List<Bug> bugsRed = new List<Bug>();
        public List<Bug> BugsRed
        {
            get { return bugsRed; }
        }

        readonly private List<Egg> eggs = new List<Egg>();
        public List<Egg> Eggs
        {
            get { return eggs; }
        }

        readonly private List<Crumbs> crumbs = new List<Crumbs>();
        public List<Crumbs> Crumbs
        {
            get { return crumbs; }
        }

        private int feedRateSpin;

        public MainWindowModel(Config _conf)
        {
            conf = _conf;

            feedRateSpin = conf.FeedRate;

            // TODO: здесь явная ошибка. Исправь позже. posX и posY генерируют
            // уникальные значения хотя это не требуется. Нужно чтобы точка
            // была уникальной.
            var posX = RandomUniqueList(0, (int)conf.MapWidth, (int)conf.BugCountTotal, random);
            var teamBluePosX = posX.GetRange(0, (int)conf.BugCountBlue);
            var teamRedPosX = posX.GetRange((int)conf.BugCountBlue, (int)conf.BugCountRed);

            var posY = RandomUniqueList(0, (int)conf.MapHeight, (int)conf.BugCountTotal, random);
            var teamBluePosY = posY.GetRange(0, (int)conf.BugCountBlue);
            var teamRedPosY = posY.GetRange((int)conf.BugCountBlue, (int)conf.BugCountRed);

            bugsBlue = ReleaseBugs(teamBluePosX.GetEnumerator(), teamBluePosY.GetEnumerator(), Bug.TeamEnum.Blue, random);
            bugsRed = ReleaseBugs(teamRedPosX.GetEnumerator(), teamRedPosY.GetEnumerator(), Bug.TeamEnum.Red, random);
        }

        private bool IsCellEmpty(int x, int y)
        {
            return GetCellObject(x, y) == null;
        }

        private IGameObject GetCellObject(int x, int y)
        {
            if (x < 0 || x >= conf.MapWidth || y < 0 || y >= conf.MapHeight)
            {
                return null;
            }

            IEnumerable<IGameObject>[] enumerables = { null, null, null, null };

            enumerables[0] =
                from bug in BugsBlue
                where bug.PosX == x && bug.PosY == y
                select bug;

            enumerables[1] =
                from bug in BugsRed
                where bug.PosX == x && bug.PosY == y
                select bug;

            enumerables[2] =
                from egg in Eggs
                where egg.PosX == x && egg.PosY == y
                select egg;

            enumerables[3] =
                from crumb in Crumbs
                where crumb.PosX == x && crumb.PosY == y
                select crumb;

            foreach (var enumerabe in enumerables)
            {
                if (enumerabe.Any())
                {
                    return enumerabe.First();
                }
            }

            return null;
        }

        private IGameObject[] GetNearestObjects(IGameObject gameObject)
        {
            IGameObject[] nearestObjects = { null, null, null, null };

            if (gameObject == null)
            {
                return nearestObjects;
            }

            nearestObjects[0] = GetCellObject(gameObject.PosX + 1, gameObject.PosY);
            nearestObjects[1] = GetCellObject(gameObject.PosX - 1, gameObject.PosY);
            nearestObjects[2] = GetCellObject(gameObject.PosX, gameObject.PosY + 1);
            nearestObjects[3] = GetCellObject(gameObject.PosX, gameObject.PosY - 1);

            return nearestObjects;
        }

        private IGameObject Hatch(Task<IGameObject> antecedent)
        {
            Egg egg = antecedent.Result as Egg;

            if(egg == null)
            {
                return null;
            }

            egg.HatchCounter -= 1;

            if (egg.IsHatching)
            {
                lock (globalLock)
                {
                    Bug bug = CreateBug(egg.PosX, egg.PosY, egg.Team, Bug.GetRandomSex());
                    bugsBlue.Add(bug);
                }
            }

            return egg;
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
                if (bug.IsDead)
                {
                    return bug;
                }

                IGameObject[] nearestObjects = GetNearestObjects(bug);

                var enemyBugs =
                    from nearestObject in nearestObjects
                    let enemyBug = nearestObject as Bug
                    where enemyBug != null && enemyBug.Team == bug.EnemyTeam
                    select enemyBug;

                if (enemyBugs.Any())
                {
                    bug.IsAtWar = true;

                    foreach (var enemyBug in enemyBugs)
                    {
                        enemyBug.Health -= bug.Strength;
                    }
                }
                else
                {
                    bug.IsAtWar = false;
                }
            }

            return bug;
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
                if (!bug.IsAtWar && !bug.IsPairing)
                {
                    IGameObject[] nearestObjects = GetNearestObjects(bug);

                    var nearestCrumbs =
                        from nearestObject in nearestObjects
                        let crumbs = nearestObject as Crumbs
                        where crumbs != null
                        select crumbs;

                    if (nearestCrumbs.Any())
                    {
                        bug.IsEating = true;

                        foreach (var crumbs in nearestCrumbs)
                        {
                            crumbs.Energy -= bug.Appetite;
                            bug.Health += bug.Appetite;
                        }
                    }
                    else
                    {
                        bug.IsEating = false;
                    }
                }
            }

            return bug;
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
                        List<Bug> collection = ((bug.Team == Bug.TeamEnum.Blue) ? BugsBlue : BugsRed);

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
                                    Egg egg = new Egg(desiredX[i], desiredY[i], bug.Team, 3);
                                    eggs.Add(egg);
                                    break;
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

                if (IsCellEmpty(desiredX, desiredY))
                {
                    bug.PosX = desiredX;
                    bug.PosY = desiredY;

                    if (random.Next() % 100 > 100 - conf.ChangeDirectionChance)
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

        private Bug ToxicHit(Task<IGameObject> antecedent)
        {
            Bug bug = antecedent.Result as Bug;

            if (bug == null)
            {
                return null;
            }

            lock (globalLock)
            {
                if (!bug.IsAtWar && !bug.IsEating && !bug.IsPairing)
                {
                    bug.Health -= conf.ToxicHit;
                }
            }

            return bug;
        }

        private void AddCrumb()
        {
            lock (globalLock)
            {
                int desiredX = random.Next() % conf.MapWidth;
                int desiredY = random.Next() % conf.MapHeight;

                if (IsCellEmpty(desiredX, desiredY))
                {
                    Crumbs crumb = new Crumbs(desiredX, desiredY, conf.CrumbEnergy);
                    crumbs.Add(crumb);
                }
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
                    .ContinueWith<IGameObject>(Move)
                    .ContinueWith<IGameObject>(ToxicHit);
                tasks.Add(task);
            }

            foreach (var bug in bugsRed)
            {
                Task task = Task
                    .Run(() => { return bug as IGameObject; })
                    .ContinueWith<IGameObject>(Fight)
                    .ContinueWith<IGameObject>(Eat)
                    .ContinueWith<IGameObject>(Pair)
                    .ContinueWith<IGameObject>(Move)
                    .ContinueWith<IGameObject>(ToxicHit);
                tasks.Add(task);
            }

            tasks.ForEach((task) => task.Wait());
            tasks.Clear();

            bugsBlue.RemoveAll((obj) => obj.DeleteMeLater);
            bugsRed.RemoveAll((obj) => obj.DeleteMeLater);

            foreach (var obj in eggs)
            {
                var task = Task
                    .Run(() => { return obj as IGameObject; })
                    .ContinueWith<IGameObject>(Hatch);
            }

            tasks.ForEach((task) => task.Wait());
            tasks.Clear();

            eggs.RemoveAll((obj) => obj.DeleteMeLater);

            feedRateSpin--;

            if (feedRateSpin <= 0)
            {
                AddCrumb();
                feedRateSpin = conf.FeedRate;
            }

            crumbs.RemoveAll((crumb) => crumb.DeleteMeLater);
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

        private Bug CreateBug(int x, int y, Bug.TeamEnum team, Bug.SexEnum sex)
        {
            if (team == Bug.TeamEnum.Blue)
            {
                return new Bug(x, y, team, sex, conf.BugHealthBlue, conf.BugStrengthBlue, conf.BugAppetiteBlue);
            }
            else if (team == Bug.TeamEnum.Red)
            {
                return new Bug(x, y, team, sex, conf.BugHealthRed, conf.BugStrengthRed, conf.BugAppetiteRed);
            }
            else
            {
                return null;
            }
        }

        private List<Bug> ReleaseBugs(IEnumerator<int> posX, IEnumerator<int> posY, Bug.TeamEnum team, Random random)
        {
            var bugs = new List<Bug>();

            for (int i = 0; i < conf.BugCountBlue; ++i)
            {
                var advanceXSuccess = posX.MoveNext();
                var advanceYSuccess = posY.MoveNext();

                if (advanceXSuccess == false || advanceYSuccess == false)
                    throw new Exception();

                Bug bug = CreateBug(posX.Current, posY.Current, team, Bug.GetRandomSex());

                bugs.Add(bug);
            }

            return bugs;
        }
    }
}
