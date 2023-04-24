using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace MCPetList
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static HttpClient mojangAPI = new() { BaseAddress = new Uri("https://api.mojang.com/users/profiles/minecraft/") };
        public static HttpClient playerdbAPI = new() { BaseAddress = new Uri("https://playerdb.co/api/player/minecraft/") };
    }
}
