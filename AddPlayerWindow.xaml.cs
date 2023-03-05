using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using System.Windows.Shapes;

namespace MCPetList
{
    /// <summary>
    /// Interaction logic for AddPlayerWindow.xaml
    /// </summary>
    public partial class AddPlayerWindow : Window
    {
        private MainWindow mainWindow;

        private string returnedUsername = string.Empty;
        string returnedUUID = string.Empty;
        public AddPlayerWindow(MainWindow mWindow)
        {
            InitializeComponent();
            mainWindow = mWindow;
            UpdateAddPlayerEnabledState();
            TextBoxSearch.Focus();
        }

        private async void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            ImageSkinDisplay.Source = await GetMCSkin(TextBoxSearch.Text);

            UpdateAddPlayerEnabledState();
        }

        private async Task<BitmapImage> GetMCSkin(string username)
        {
            LabelNameplate.Visibility = Visibility.Visible;
            if (string.IsNullOrWhiteSpace(username))
            {

                LabelNameplate.Content = "MHF_Steve";
                return new BitmapImage(new Uri(@"resources\defaultskin.png", UriKind.Relative));
            }

            try
            {
                var userInfoFromAPI = await MainWindow.sharedClient.GetStringAsync(username);
                if (userInfoFromAPI.Length == 0)
                {
                    MessageBox.Show("User does not exist.");
                    LabelNameplate.Content = "MHF_Steve";
                    return new BitmapImage(new Uri(@"resources\defaultskin.png", UriKind.Relative));
                }
                else
                {
                    var jsonUserInfo = JsonDocument.Parse(userInfoFromAPI);

                    returnedUsername = jsonUserInfo.RootElement.TryGetProperty("name", out var nameElement)
                        ? nameElement.ToString()
                        : username;

                    returnedUUID = jsonUserInfo.RootElement.TryGetProperty("id", out var uuidElement)
                        ? uuidElement.ToString()
                        : "";

                    LabelNameplate.Content = returnedUsername;
                    return new BitmapImage(new Uri("https://crafatar.com/renders/body/" + returnedUUID +
                                                   "?default=MHF_Steve/MHF_Alex&overlay"));
                }
            }
            catch (Exception)
            {
                returnedUsername = username;

                returnedUUID = "";

                LabelNameplate.Content = returnedUsername;
                return new BitmapImage(new Uri(@"resources\defaultskin.png", UriKind.Relative));
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.players.Any(p => p.Username == returnedUsername))
            {
                MessageBox.Show("You have already added that player");
            }
            else
            {
                mainWindow.players.Add(new Player(returnedUsername, returnedUUID));
                mainWindow.GenerateExpanders();
                mainWindow.UpdateAddPetButtonEnabledState();
                this.Close();
            }
        }

        private void UpdateAddPlayerEnabledState()
        {
            ButtonAdd.IsEnabled = (returnedUsername != String.Empty);
        }

        private void TextBoxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox?.SelectAll();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
