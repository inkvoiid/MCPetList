using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPetList
{
    public class Player
    {
        public string Username { get; set; }
        public string UUID { get; set; }
        public List<Pet> Pets { get; set; }
        public Player(string uname, string uuid) 
        {
            Username = uname;
            UUID = uuid;
            Pets = new List<Pet>();
        }
    }
}
