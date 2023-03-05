using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for AddPetWindow.xaml
    /// </summary>
    public partial class AddPetWindow : Window
    {
        List<string> petTypes = new List<string> { "Allay", "Axolotl", "Camel", "Cat", "Dog", "Donkey", "Fox", "Horse", "Llama", "Mule", "Ocelot", "Parrot" };
        Color[] collarColours = new Color[] { Colors.White, Colors.LightGray, Colors.Gray, Colors.Black, Colors.Brown, Colors.Red, Colors.Orange, Colors.Yellow, Colors.Lime, Colors.Green, Colors.Cyan, Colors.LightBlue, Colors.Blue, Colors.Purple, Colors.Magenta, Colors.Pink };
        public AddPetWindow(MainWindow mWindow)
        {
            InitializeComponent();
        }
    }
}
