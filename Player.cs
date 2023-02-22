using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPetList
{
    internal class Player
    {
        public string Username { get; set; }
        public string UUID { get; set; }
        public List<Pet> Pets { get; set; }
        public Player(string uname) { }
    }
}
