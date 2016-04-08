using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;
using Oyosoft.AgenceImmobiliere.Core.Service;
using Oyosoft.AgenceImmobiliere.Core.Tools;

namespace Oyosoft.AgenceImmobiliere.WpfClient
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Core.ViewModels.Connection._dbDefaultDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            Core.ViewModels.Connection._dbDirectoryPath = WpfClient.Properties.Settings.Default.DATABASE_DIRECTORY_PATH;
            Core.ViewModels.Connection._dbFileName = WpfClient.Properties.Settings.Default.DATABASE_FILE_NAME;
            Core.ViewModels.Connection._sqlitePlatform = new SQLite.Net.Platform.Generic.SQLitePlatformGeneric();
            Core.ViewModels.Connection._endpointConfigurationName = WpfClient.Properties.Settings.Default.ENDPOINT_NAME;
            Core.ViewModels.Connection._endpointConfigurationAddress = WpfClient.Properties.Settings.Default.ENDPOINT_ADRESS;
            Core.ViewModels.BienImmobilier.ListAndDetails.itemsCountOnPage = 15;
        }
    }
}
