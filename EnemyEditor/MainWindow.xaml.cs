using Rogue;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace EnemyEditor
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Interaction logic for MainWindow.xaml
        /// </summary>

        public MainWindow()
        {
          
            InitializeComponent();
        }

        private void ButtonAddName_Click(object sender, RoutedEventArgs e)
        {
            bool canSend = false;
            errorLabel.Content = " ";
            if (!string.IsNullOrWhiteSpace(txtName.Text) && !lstNames.Items.Contains(txtName.Text))
            {
                canSend = true;
                
                //Save to JSON
            }

            int spriteID;

            if (Int32.TryParse(txtTileId.Text, out spriteID))
            {
                if (spriteID >= 0)
                {
                   
                    //Save to JSON
                }
            }
            else
            {
                errorLabel.Content = "Give Number that is zero or larger";
                txtTileId.Clear();
            }
            
            int hitPoints;

            if (Int32.TryParse(txtHitPoints.Text, out hitPoints))
            {
                if (hitPoints > 0)
                {
                  
                    //Save to JSON
                }
            }
            else
            {
                errorLabel.Content = "Give Number that is larger than zero";
                txtHitPoints.Clear();
            }

            if (canSend)
            {
                Rogue.Enemy addEnemy = new Rogue.Enemy(txtName.Text, spriteID, hitPoints);
             
                lstNames.Items.Add(addEnemy);

                txtHitPoints.Clear();
                txtTileId.Clear();
                txtName.Clear();
            }

        }

        private void ButtonAddJSON_Click(object sender, RoutedEventArgs e)
        {
            
            // Katso kuinka iso taulukko tarvitaan
            int EnemyCount = lstNames.Items.Count;

            List<Rogue.Enemy> tempList = new List<Rogue.Enemy>();


            string filename = "enemies.json";
            for (int i = 0; i < lstNames.Items.Count; i++)
            {
                Enemy enemy = (Enemy)lstNames.Items[i];
                tempList.Add(enemy);
            }
            string enemiesArrayJSON = JsonConvert.SerializeObject(tempList);

            File.WriteAllText(filename, enemiesArrayJSON);

            errorLabel.Content = "Write OK!";
            
        }

   
    }
}