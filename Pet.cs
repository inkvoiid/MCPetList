using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPetList
{
    internal abstract class Pet
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Player Owner { get; set; }

        public Pet(string name, string type, Player owner)
        {
            Name = name;
            Type = type;
            Owner = owner;
        }

    }

    internal class Allay : Pet
    {
        public Allay(string name, string type, Player owner) : base(name, type, owner)
        {
            Name = name;
            Type = type;
            Owner = owner;
        }
    }

    internal class Axolotl : Pet
    {
        public Axolotl(string name, string type, Player owner) : base(name, type, owner)
        {
            Name = name;
            Type = type;
            Owner = owner;
        }
    }

    internal class Camel : Pet
    {
        public Camel(string name, Player owner) : base(name, "Camel", owner)
        {
            Name = name;
            Owner = owner;
        }
    }

    internal class Cat : Pet
    {
        public enum CollarColourEnum { White, LightGray, Gray, Black, Brown, Red, Orange, Yellow, Lime, Green, Cyan, LightBlue, Blue, Purple, Magenta, Pink }
        public bool IsBaby {get; set;}
        public CollarColourEnum Collar {get; set;}
        public Cat(string name, string type, CollarColourEnum collar, bool isBaby, Player owner) : base(name, type, owner)
        {
            Name = name;
            Type = type;
            Collar = collar;
            IsBaby = isBaby;
            Owner = owner;
        }
    }

        internal class Dog : Pet
    {
        public enum CollarColourEnum { White, LightGray, Gray, Black, Brown, Red, Orange, Yellow, Lime, Green, Cyan, LightBlue, Blue, Purple, Magenta, Pink }
        public bool IsBaby {get; set;}
        public CollarColourEnum Collar {get; set;}
        public Dog(string name, string type, CollarColourEnum collar, bool isBaby, Player owner) : base(name, type, owner)
        {
            Name = name;
            Type = type;
            Collar = collar;
            IsBaby = isBaby;
            Owner = owner;
        }
    }
}
