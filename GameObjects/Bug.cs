using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugWars.GameObjects
{
    public class Bug : GameObject
    {
        private static readonly Random random = new Random(Environment.TickCount);

        // Кок получить произвольное значение из перечисления:
        // https://stackoverflow.com/questions/3132126/how-do-i-select-a-random-value-from-an-enumeration
        private static Array sex = Enum.GetValues(typeof(Bug.SexEnum));
        private static Array direction = Enum.GetValues(typeof(Bug.DirectionEnum));

        public enum SexEnum { Male = 0, Female = 1 };
        public enum TeamEnum { Red = 0, Blue = 1 };
        public enum DirectionEnum { Up, Down, Lef, Right };

        public TeamEnum Team { get; set; }
        public TeamEnum EnemyTeam { get { return Team == TeamEnum.Blue ? TeamEnum.Red : TeamEnum.Blue; } }

        public SexEnum Sex { get; set; }
        public SexEnum OppositeSex { get { return Sex == SexEnum.Male ? SexEnum.Female : SexEnum.Male; } }

        public DirectionEnum Direction { get; set; }

        public bool IsPairing { get; set; }

        private int pairingCounter;
        public int PairingCounter
        {
            get { return pairingCounter; }
            set { pairingCounter = value > 0 ? value : 0; }
        }

        private int fertilityCounter;
        public int FertilityCounter
        {
            get { return fertilityCounter; }
            set { fertilityCounter = value > 0 ? value : 0; }
        }

        public bool IsAtWar { get; set; }
        public bool IsEating { get; set; }

        public int Health { get; set; }
        public uint Speed { get; set; }
        public int Strength { get; set; }
        public uint Appetite { get; set; }

        public Bug(int x, int y, Bug.TeamEnum team, Bug.SexEnum sex, int health, int strength)
        {
            PosX = x;
            PosY = y;
            Team = team;
            Sex = sex;
            Health = health;
            Strength = strength;

            IsAtWar = false;
            IsEating = false;
            IsPairing = false;
            Direction = Bug.GetRandomDirection();
        }

        public static DirectionEnum GetRandomDirection()
        {
            return (Bug.DirectionEnum)direction.GetValue(random.Next(direction.Length));
        }

        public static SexEnum GetRandomSex()
        {
            return (Bug.SexEnum)sex.GetValue(random.Next(sex.Length));
        }
    }
}
