using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace MCPetList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Player> players = new List<Player>();
        public static HttpClient sharedClient = new()
        {
            BaseAddress = new Uri("https://api.mojang.com/users/profiles/minecraft/"),
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Img.Source = await GetAvatar(Interaction.InputBox("Enter your username", "Enter username", "jeb_"));
            GenerateExpanders();
        }

        public async void GenerateExpanders()
        {
            MainPanel.Children.Clear();
            foreach (var player in players)
            {
                Image playerHead = new Image
                {
                    Source = player.UUID == "" ? new BitmapImage(new Uri(@"resources\defaultavatar.png", UriKind.Relative)) : await GetAvatar(player.Username),
                    Height = 25
                };
                TextBlock headerText = new TextBlock
                {
                    Text = player.Username,
                    Style = this.FindResource("MCTextStyle") as Style,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };
                StackPanel userExpanderHeader = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children = { playerHead, headerText }
                };
                Expander userExpander = new Expander
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 158, 158, 158)),
                    Header = userExpanderHeader
                };
                
                MainPanel.Children.Add(userExpander);

                UpdateAddPetButtonEnabledState();
            }
        }

        private async Task<BitmapImage> GetAvatar(string username)
        {
            // Check for local file
            var fileName = $"{username}.png";
            var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/avatars/", fileName);

            if (File.Exists(localPath))
            {
                BitmapImage avatar = new BitmapImage();
                avatar.BeginInit();
                avatar.CacheOption = BitmapCacheOption.OnLoad;
                avatar.UriSource = new Uri(localPath);
                avatar.EndInit();
                return avatar;
            }
            else
            {
                var userInfoFromAPI = await sharedClient.GetStringAsync(username);
                if (userInfoFromAPI.Length == 0)
                {
                    MessageBox.Show("User does not exist.");
                    return new BitmapImage(new Uri(@"resources\defaultavatar.png", UriKind.Relative));
                }

                var uuid = JsonDocument.Parse(userInfoFromAPI).RootElement.GetProperty("id").GetString();

                var webPath = $"https://crafatar.com/avatars/{uuid}?default=MHF_Steve/MHF_Alex&overlay";

                if(Path.GetDirectoryName(localPath) != String.Empty)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));

                    using (HttpClient client = new HttpClient())
                    {
                        byte[] imageBytes = await client.GetByteArrayAsync(webPath);
                        File.WriteAllBytes(localPath, imageBytes);
                    }
                    return new BitmapImage(new Uri(webPath));
                }
                else
                {
                    return new BitmapImage(new Uri(@"resources\defaultavatar.png", UriKind.Relative));
                }
            }
        }

        private void ButtonAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            AddPlayerWindow addPlayerWindow = new AddPlayerWindow(this)
            {
                Owner = WindowMain,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            addPlayerWindow.Show();
        }

        private void ButtonAddPet_Click(object sender, RoutedEventArgs e)
        {
            UpdateAddPetButtonEnabledState();
            AddPetWindow addPetWindow = new AddPetWindow(this)
            {
                Owner = WindowMain,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            addPetWindow.Show();
        }

        public void UpdateAddPetButtonEnabledState()
        {
            ButtonAddPet.IsEnabled = (players.Count > 0);
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            GenerateExpanders();
            UpdateAddPetButtonEnabledState();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SavePlayersToJson(players);
        }

        private void SavePlayersToJson(List<Player> playersToSave)
        {
            var saveJsonWindow = new SaveFileDialog { Filter= "JSON Files (*.json)|*.json" };
            if (saveJsonWindow.ShowDialog() == true)
            {
                var jsonString = System.Text.Json.JsonSerializer.Serialize(playersToSave, new JsonSerializerOptions { WriteIndented = true});
                File.WriteAllText(saveJsonWindow.FileName, jsonString);
            }
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            var openJsonWindow = new OpenFileDialog { Filter = "JSON Files (*.json)|*.json" };
            if (openJsonWindow.ShowDialog() == true)
            {
                string jsonFromFile = File.ReadAllText(openJsonWindow.FileName);
                if(jsonFromFile != null)
                {
                    players = JsonConvert.DeserializeObject<List<Player>>(jsonFromFile);
                    GenerateExpanders();
                }
                else
                {
                    MessageBox.Show("Couldn't open that file.");
                }

            }


        }
    }
}
