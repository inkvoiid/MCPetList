using System;
using System.Collections.Generic;
using System.Configuration;
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
        private string currentFile = string.Empty;
        public List<Player> players = new List<Player>();

        public MainWindow()
        {
            InitializeComponent();
        }

/// <summary>
/// The function loads a previously opened file and generates expanders based on the data in the file.
/// </summary>
/// <param name="sender">The object that raised the event, in this case, the Window that was
/// loaded.</param>
/// <param name="RoutedEventArgs">RoutedEventArgs is an event data class that is used to pass event data
/// to an event handler when an event is raised. It contains information about the event, such as the
/// source of the event and any additional data related to the event. In this case, the Window_Loaded
/// event is raised when the</param>

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string lastFileOpened = ConfigurationManager.AppSettings["LastOpenedFile"] ?? "";
            
            if (!string.IsNullOrEmpty(lastFileOpened))
            {
                string jsonFromFile = File.ReadAllText(lastFileOpened);

                if (!string.IsNullOrEmpty(jsonFromFile))
                {
                    currentFile = lastFileOpened;
                    players = JsonConvert.DeserializeObject<List<Player>>(jsonFromFile);
                    GenerateExpanders();
                }
            }
            GenerateExpanders();
        }

/// <summary>
/// This function generates expanders for each player and adds them to the main panel.
/// </summary>
        public async void GenerateExpanders()
        {
            MainPanel.Children.Clear();
            foreach (var player in players)
            {
                Image playerHead = new Image
                {
                    Source = string.IsNullOrEmpty(player.UUID) ? new BitmapImage(new Uri(@"resources\defaultavatar.png", UriKind.Relative)) : await GetAvatar(player.Username),
                    Height = 25
                };
                TextBlock headerText = new TextBlock
                {
                    Text = player.Username,
                    Style = (Style)this.FindResource("MCTextStyle"),
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

/// <summary>
/// This function retrieves a user's avatar image either from a local file or from an API, and returns
/// it as a BitmapImage object.
/// </summary>
/// <param name="username">The username of the user whose avatar is being retrieved.</param>
/// <param name="skipLocalFile">Whether or not the method should skip checking the local files, defaults to false.</param>
/// <returns>
/// The method returns a `Task` that resolves to a `BitmapImage`.
/// </returns>
        private async Task<BitmapImage> GetAvatar(string username, bool skipLocalFile = false)
        {
            // Check for local file
            var fileName = $"{username}.png";
            var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/avatars/", fileName);

            if (File.Exists(localPath) && (skipLocalFile == false))
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
                try
                {
                    var player = players.FirstOrDefault(player => player.Username == username);
                    string uuid;
                    if (player != null)
                    {
                        uuid = player.UUID;
                    }

                    else {
                        var userInfoFromAPI = await App.mojangAPI.GetStringAsync(username);
                        if (userInfoFromAPI.Length == 0)
                        {
                            MessageBox.Show("User does not exist.");
                            return new BitmapImage(new Uri(@"resources\defaultavatar.png", UriKind.Relative));
                        }

                        uuid = JsonDocument.Parse(userInfoFromAPI).RootElement.GetProperty("id").GetString();

                    }

                    var webPath = $"https://crafatar.com/avatars/{uuid}?default=MHF_Steve/MHF_Alex&overlay";

                    if (Path.GetDirectoryName(localPath) != String.Empty)
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
                catch
                {
                    MessageBox.Show("User does not exist.");
                    return new BitmapImage(new Uri(@"resources\defaultavatar.png", UriKind.Relative));
                }
            }
        }

        /// <summary>
        /// This function opens a save file dialog window and saves a list of players as a JSON file with an
        /// option to write it indented.
        /// </summary>
        /// <param name="playersToSave">A list of Player objects that need to be saved in a JSON file.</param>
        private void OpenSavePlayerJsonWindow(List<Player> playersToSave)
        {
            var saveJsonWindow = new SaveFileDialog { Filter= "JSON Files (*.json)|*.json" };
            if (saveJsonWindow.ShowDialog() == true)
            {
                var jsonString = System.Text.Json.JsonSerializer.Serialize(playersToSave, new JsonSerializerOptions { WriteIndented = true});
                File.WriteAllText(saveJsonWindow.FileName, jsonString);
            }
        }

        private void SaveJson(List<Player> playersToSave)
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(playersToSave, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(currentFile, jsonString);
        }

        // Button Code

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
            SaveJson(players);
        }

        private void ButtonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            OpenSavePlayerJsonWindow(players);
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            var openJsonWindow = new OpenFileDialog { Filter = "JSON Files (*.json)|*.json" };
            if (openJsonWindow.ShowDialog() == true)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                currentFile = openJsonWindow.FileName;
                config.AppSettings.Settings["LastOpenedFile"].Value = currentFile;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                string jsonFromFile = File.ReadAllText(openJsonWindow.FileName);
                if(jsonFromFile != "")
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

        private void ButtonRefreshPlayerData_Click(object sender, RoutedEventArgs e)
        {
            foreach (var player in players)
            {
                if (string.IsNullOrEmpty(player.UUID))
                {
                    player.GetUUID();
                }

                player.GetUsername();
                GetAvatar(player.Username, true);
            }

            GenerateExpanders();
            UpdateAddPetButtonEnabledState();
        }
    }
}
