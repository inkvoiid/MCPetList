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

        public Pet(string nme, string typ, Player ownr)
        {
            Name = nme;
            Type = typ;
            Owner = ownr;
        }

    }

    internal class Allay : Pet
    {
        public Allay(string nme, string typ, Player ownr) : base(nme, typ, ownr)
        {
            Name = nme;
            Type = typ;
            Owner = ownr;
        }
    }
}
