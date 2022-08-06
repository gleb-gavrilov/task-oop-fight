using System;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {
            Arena arena = new Arena();

            arena.ChooseFighters();

            Console.Clear();
            Console.WriteLine("Бойцы готовы, песок прогрет, зрители заняли свои места...");

            arena.Fight();
        }
    }

    class Arena
    {
        private NPC _leftFighter;
        private NPC _rightFighter;

        public void ChooseFighters()
        {
            Console.WriteLine("Выберете бойца в левой части арены:");
            int leftFigherNumber = ChooseFigter();
            
            Console.WriteLine("Выберете бойца в правой части арены:");
            int rightFigherNumber = ChooseFigter();

            _leftFighter = CreateFighter(leftFigherNumber);
            _rightFighter = CreateFighter(rightFigherNumber);
        }

        public void Fight()
        {
            while (_leftFighter.Health >= 0 && _rightFighter.Health >= 0)
            {
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                float attackLeftFighter = _leftFighter.MakeDamage();
                float attackRightFighter = _rightFighter.MakeDamage();

                _rightFighter.TakeDamage(attackLeftFighter);
                _leftFighter.TakeDamage(attackRightFighter);

                Console.WriteLine($"Боец в левом углу атакует на {attackLeftFighter} урона. Жизней остается {_leftFighter.Health}");
                Console.WriteLine($"Боец в правом углу атакует на {attackRightFighter} урона. Жизней остается {_rightFighter.Health}");
            }

            if (_leftFighter.Health <= 0 && _rightFighter.Health <= 0)
            {
                Console.WriteLine("Ничья! Оба бойца пали!");
            }
            else if (_leftFighter.Health > 0)
            {
                Console.WriteLine("Победил боец в левом углу!");
            }
            else
            {
                Console.WriteLine("Победил боец в правом углу!");
            }
        }

        public void ShowFightersInfo()
        {
            Console.WriteLine("1. Warrior - При низком уровне здоровья впадает в адскую ярость");
            Console.WriteLine("2. Mage - Есть 20% шанс выпустить дополнительный файрболл");
            Console.WriteLine("3. Assassin - Есть 30% шанс уклониться от атаки");
            Console.WriteLine("4. Vampire - Каждый удар восстанавливает себе здоровье в размере 20% от наносимого урона");
            Console.WriteLine("5. Berserker - Каждый 3-ий удр делает крит и повышает себе броню");
        }

        private NPC CreateFighter(int fighterNumber)
        {
            Random random = new Random();

            string fighterName = $"Fighter # {random.Next(1, 10000)}";
            NPC fighter = new Warrior("DUMMY", 1, 1, 1, new LightArmor());

            switch (fighterNumber)
            {
                case 1:
                    fighter = new Warrior(fighterName, 2000, 200, 20, new HeavyArmor());
                    break;

                case 2:
                    fighter = new Mage(fighterName, 1000, 100, 10, new LightArmor());
                    break;

                case 3:
                    fighter = new Assassin(fighterName, 1500, 100, 15, new MediumArmor());
                    break;

                case 4:
                    fighter = new Vampire(fighterName, 1800, 110, 15, new MediumArmor());
                    break;

                case 5:
                    fighter = new Berserker(fighterName, 1300, 200, 0, new LightArmor());
                    break;
            }

            return fighter;
        }

        private int ChooseFigter()
        {
            Random random = new Random();

            ShowFightersInfo();

            int fighterNumber = UserUtils.GetNumber();

            if (fighterNumber <= 0 || fighterNumber > 5)
            {
                fighterNumber = random.Next(1, 6);
                Console.WriteLine($"Такого бойца у нас нет, мы сами выберем вам бойца под номером {fighterNumber}");
            }

            return fighterNumber;
        }
    }

    class Armor
    {
        public int AbsorbDamagePerArmor { get; protected set; }
    }

    class LightArmor : Armor
    {
        public LightArmor()
        {
            AbsorbDamagePerArmor = 2;
        }
    }

    class MediumArmor : Armor
    {
        public MediumArmor()
        {
            AbsorbDamagePerArmor = 4;
        }
    }

    class HeavyArmor : Armor
    {
        public HeavyArmor()
        {
            AbsorbDamagePerArmor = 6;
        }
    }

    abstract class NPC
    {
        public string Name { get; protected set; }
        public float Health { get; protected set; }
        public float Damage { get; protected set; }
        public int Armor { get; protected set; }
        public Armor ArmorType { get; protected set; }

        public virtual void TakeDamage(float damage)
        {
            ExecuteEventsBeforeTakeDamage();

            if (damage > Armor)
            {
                Health -= (damage - Armor);
            }
            else
            {
                Console.WriteLine("Броня слишком прочная, ее не пробить");
            }
        }

        public virtual float MakeDamage()
        {
            return Damage;
        }

        protected virtual void ExecuteEventsBeforeTakeDamage() { }
    }

    class Warrior : NPC
    {
        private bool _isFrenzyEnabled = false;
        private int _minimunHealthForFrenzy = 200;
        
        public Warrior(string name, int health, int damage, int armor, Armor armorType)
        {
            Name = name;
            Health = health;
            Damage = damage;
            Armor = armorType.AbsorbDamagePerArmor * armor;
            ArmorType = armorType;
        }
                
        protected override void ExecuteEventsBeforeTakeDamage()
        {
            if (_minimunHealthForFrenzy > Health && _isFrenzyEnabled == false)
            {
                EnableFrenzy();
            }
        }

        private void EnableFrenzy()
        {
            Armor += 30;
            Health += 300;
            Damage += 100;

            _isFrenzyEnabled = true;

            Console.WriteLine($"Боец {Name} впадает в безумие!");
        }
    }

    class Mage : NPC
    {
        private int _minimumFireBallChance = 1;
        private int _maximumFireBallChance = 6;
        private int _fireBallChance = 2;
        private int _fireBallDamage = 150;

        public Mage(string name, int health, int damage, int armor, Armor armorType)
        {
            Name = name;
            Health = health;
            Damage = damage;
            Armor = armorType.AbsorbDamagePerArmor* armor;
            ArmorType = armorType;
        }

        public override float MakeDamage()
        {
            if (isActivateFireBall())
            {
                Console.WriteLine("Хо-хо! Во врага летит файрбол!");

                return Damage + _fireBallDamage;
            }

            return Damage;
        }

        private bool isActivateFireBall()
        {
            Random random = new Random();

            if (random.Next(_minimumFireBallChance, _maximumFireBallChance) == _fireBallChance)
            {
                return true;
            }

            return false;
        }

    }

    class Assassin : NPC
    {
        private int _minimumAvoidChance = 1;
        private int _maximumAvoidChance = 4;
        private int _avoidChance = 3;

        public Assassin(string name, int health, int damage, int armor, Armor armorType)
        {
            Name = name;
            Health = health;
            Damage = damage;
            Armor = armorType.AbsorbDamagePerArmor * armor;
            ArmorType = armorType;
        }

        public override void TakeDamage(float damage)
        {
            if (isActivateAvoid())
            {
                Console.WriteLine("Успешное уклонение от атаки");
            }
            else
            {
                Health -= (damage - Armor);
            }
        }

        private bool isActivateAvoid()
        {
            Random random = new Random();

            if (random.Next(_minimumAvoidChance, _maximumAvoidChance) == _avoidChance)
            {
                return true;
            }

            return false;
        }
    }

    class Vampire : NPC
    {
        private int _percentsHealthSteal = 20;

        public Vampire(string name, int health, int damage, int armor, Armor armorType)
        {
            Name = name;
            Health = health;
            Damage = damage;
            Armor = armorType.AbsorbDamagePerArmor * armor;
            ArmorType = armorType;
        }

        public override float MakeDamage()
        {
            float recoveryHealth = (Damage / 100) *_percentsHealthSteal;
            Health += recoveryHealth;

            return Damage;
        }
    }

    class Berserker : NPC
    {
        private int _punchCounter = 0;
        private int _critRate = 3;
        private int _damageMultiply = 2;
        private int _armorUpgradeForMove = 10;

        public Berserker(string name, int health, int damage, int armor, Armor armorType)
        {
            Name = name;
            Health = health;
            Damage = damage;
            Armor = armorType.AbsorbDamagePerArmor * armor;
            ArmorType = armorType;
        }

        public override float MakeDamage()
        {
            _punchCounter++;

            if (_punchCounter % _critRate == 0)
            {
                Console.WriteLine("Критический урон!");

                return Damage * _damageMultiply;
            }

            return Damage;
        }

        protected override void ExecuteEventsBeforeTakeDamage()
        {
            Armor += _armorUpgradeForMove;
        }
    }

    class UserUtils
    {
        public static void WaitUser()
        {
            Console.WriteLine("Для продолжения нажмите любую клавишу...");
            Console.ReadKey();
        }

        public static int GetNumber()
        {
            int number = 0;
            bool isNumber = false;

            while (isNumber == false)
            {
                isNumber = Int32.TryParse(Console.ReadLine(), out number);

                if (isNumber == false)
                {
                    Console.WriteLine("Нужно ввести число!");
                }
            }

            return number;
        }
    }
}
