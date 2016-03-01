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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Oyosoft.AgenceImmobiliere.WpfClient
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            Core.ViewModels.Connection._dbDefaultDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            Core.ViewModels.Connection._dbDirectoryPath = Properties.Settings.Default.DATABASE_DIRECTORY_PATH;
            Core.ViewModels.Connection._dbFileName = Properties.Settings.Default.DATABASE_FILE_NAME;
            Core.ViewModels.Connection._sqlitePlatform = new SQLite.Net.Platform.Generic.SQLitePlatformGeneric();
            Core.ViewModels.Connection._endpointConfigurationName = Properties.Settings.Default.ENDPOINT_NAME;
            Core.ViewModels.Connection._endpointConfigurationAddress = Properties.Settings.Default.ENDPOINT_ADRESS;
            InitializeComponent();
        }
        
    }
}
