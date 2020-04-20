using System;
using System.Drawing; // Point
using System.Threading;
using System.Diagnostics; // Debug.Assert
using System.Collections.Generic; // List<>
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

            var pos = GetRandomPoints();
            var teamBluePos = pos.GetRange(0, conf.BugCountBlue);
            var teamRedPos = pos.GetRange(conf.BugCountBlue, conf.BugCountRed);

            //Debug.Assert(teamBluePos.Count == conf.BugCountBlue);
            //Debug.Assert(teamRedPos.Count == conf.BugCountRed);

            bugsBlue = ReleaseBugs(teamBluePos.GetEnumerator(), Bug.TeamEnum.Blue, random);
            bugsRed = ReleaseBugs(teamRedPos.GetEnumerator(), Bug.TeamEnum.Red, random);
        }

        private List<Point> GetRandomPoints()
        {
            List<Point> points = new List<Point>();

            for (int i = 0; i < conf.BugCountTotal; ++i)
            {
                Point point = new Point();

                do
                {
                    point.X = random.Next(0, conf.MapWidth);
                    point.Y = random.Next(0, conf.MapHeight);
                } while (points.Contains(point));

                points.Add(point);
            }

            return points;
        }

        private bool IsCellEmpty(int x, int y)
        {
            return GetCellObject(x, y) == null;
        }

        private IGameObject GetCellObject(int x, int y)
        {
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

            lock (globalLock)
            {
                egg.HatchCounter -= 1;

                if (egg.IsHatched)
                {
                    lock (globalLock)
                    {
                        Bug bug = CreateBug(egg.PosX, egg.PosY, egg.Team, Bug.GetRandomSex());
                        var bugs = (egg.Team == Bug.TeamEnum.Blue) ? bugsBlue : bugsRed;
                        bugs.Add(bug);
                    }
                }

                return egg;
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
                            bug.Health += crumbs.TakeEnergy(bug.Appetite);
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

                            //Debug.Assert(bug.PairingCounter == 0);
                            //Debug.Assert(bug.FertilityCounter == 0);

                            //Debug.Assert(otherBug.PairingCounter == 0);
                            //Debug.Assert(otherBug.FertilityCounter == 0);

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
                        //Debug.Assert(bug.PairingCounter == 0);

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

                if (desiredX >= 0 && desiredX < conf.MapWidth &&
                    desiredY >= 0 && desiredY < conf.MapHeight &&
                    IsCellEmpty(desiredX, desiredY) == true)
                {
                    bug.PosX = desiredX;
                    bug.PosY = desiredY;

                    if (random.Next(0, 100) > 100 - conf.ChangeDirectionChance)
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

            bugsBlue.RemoveAll((obj) => obj.IsDead);
            bugsRed.RemoveAll((obj) => obj.IsDead);

            foreach (var obj in eggs)
            {
                var task = Task
                    .Run(() => { return obj as IGameObject; })
                    .ContinueWith<IGameObject>(Hatch);
            }

            tasks.ForEach((task) => task.Wait());
            tasks.Clear();

            eggs.RemoveAll((obj) => obj.IsHatched);

            feedRateSpin--;

            if (feedRateSpin <= 0)
            {
                AddCrumb();
                feedRateSpin = conf.FeedRate;
            }

            crumbs.RemoveAll((crumb) => crumb.IsEmpty);
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

        private List<Bug> ReleaseBugs(IEnumerator<Point> pos, Bug.TeamEnum team, Random random)
        {
            var bugs = new List<Bug>();

            for (int i = 0; i < conf.BugCountBlue; ++i)
            {
                var advanceSuccess = pos.MoveNext();

                if (advanceSuccess == false || advanceSuccess == false)
                    throw new Exception();

                Bug bug = CreateBug(pos.Current.X, pos.Current.Y, team, Bug.GetRandomSex());

                bugs.Add(bug);
            }

            return bugs;
        }
    }
}
