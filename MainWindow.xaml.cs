using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
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
using Path = System.IO.Path;

namespace MCPetList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string,string> players = new Dictionary<string,string>();
        public static HttpClient sharedClient = new()
        {
            BaseAddress = new Uri("https://api.mojang.com/users/profiles/minecraft/"),
        };

        public MainWindow()
        {
            InitializeComponent(); 
            UpdateAddPetButtonEnabledState();

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
                Expander userExpander = new Expander();
                StackPanel userExpanderHeader = new StackPanel();
                Image playerHead = new Image();
                TextBlock headerText = new TextBlock();

                // Make the image and name appear side by side
                userExpanderHeader.Orientation = Orientation.Horizontal;
                playerHead.Source = player.Value == "" ? new BitmapImage(new Uri(@"resources\defaultavatar.png", UriKind.Relative)) : await GetAvatar(player.Key);
                playerHead.Height = 25;
                headerText.Text = player.Key;
                headerText.VerticalAlignment = VerticalAlignment.Center;
                headerText.Margin = new Thickness(5);

                userExpanderHeader.Children.Add(playerHead);
                userExpanderHeader.Children.Add(headerText);

                userExpander.Header = userExpanderHeader;
                MainPanel.Children.Add(userExpander);
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

                Directory.CreateDirectory(Path.GetDirectoryName(localPath));

                using (HttpClient client = new HttpClient())
                {
                    byte[] imageBytes = await client.GetByteArrayAsync(webPath);
                    File.WriteAllBytes(localPath, imageBytes);
                }
                return new BitmapImage(new Uri(webPath));
            }
        }

        private void ButtonAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            AddPlayerWindow addPlayerWindow = new AddPlayerWindow(this);
            addPlayerWindow.Owner = WindowMain;
            addPlayerWindow.SizeToContent = SizeToContent.WidthAndHeight;
            addPlayerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addPlayerWindow.Show();
        }

        private void ButtonAddPet_Click(object sender, RoutedEventArgs e)
        {
            UpdateAddPetButtonEnabledState();
            AddPetWindow addPetWindow = new AddPetWindow(this);
            addPetWindow.Owner = WindowMain;
            addPetWindow.SizeToContent = SizeToContent.WidthAndHeight;
            addPetWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
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
    }
}
