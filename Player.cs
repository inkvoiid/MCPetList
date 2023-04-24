using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

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

        public void GetUUID()
        {
            if (string.IsNullOrEmpty(UUID))
            {
                var userInfoFromAPI = App.mojangAPI.GetStringAsync(Username);

                var jsonUserInfo = JsonDocument.Parse(userInfoFromAPI.Result);
                jsonUserInfo.RootElement.TryGetProperty("id", out var uuidElement);
                UUID = uuidElement.ToString();
            }
        }

        public void GetUsername()
        {
            if (!string.IsNullOrEmpty(UUID))
            {
                var userInfoFromAPI = App.playerdbAPI.GetStringAsync(UUID);
                var jsonUserInfo = JsonDocument.Parse(userInfoFromAPI.Result);
                
                if (jsonUserInfo.RootElement.TryGetProperty("data", out var dataElement) &&
                    dataElement.TryGetProperty("player", out var playerElement) &&
                    playerElement.TryGetProperty("username", out var usernameElement))
                {
                    Username = usernameElement.ToString();
                }
            }
        }
    }
}
