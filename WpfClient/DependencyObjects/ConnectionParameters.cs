using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.WpfClient.DependencyObjects
{
    class ConnectionParameters
    {

        public static readonly DependencyProperty DbDefaultDirectoryPathProperty = DependencyProperty.RegisterAttached("DbDefaultDirectoryPath", typeof(string), typeof(ConnectionParameters));
        public static string GetDbDefaultDirectoryPath(DependencyObject obj)
        {
            return (string)obj.GetValue(DbDefaultDirectoryPathProperty);
        }
        public static void SetDbDefaultDirectoryPath(DependencyObject obj, string value)
        {
            obj.SetValue(DbDefaultDirectoryPathProperty, value);
            Connection._dbDefaultDirectoryPath = value;
        }


        public static readonly DependencyProperty DbDirectoryPathProperty = DependencyProperty.RegisterAttached("DbDirectoryPath", typeof(string), typeof(ConnectionParameters));
        public static string GetDbDirectoryPath(DependencyObject obj)
        {
            return (string)obj.GetValue(DbDirectoryPathProperty);
        }
        public static void SetDbDirectoryPath(DependencyObject obj, string value)
        {
            obj.SetValue(DbDirectoryPathProperty, value);
            Connection._dbDirectoryPath = value;
        }


        public static readonly DependencyProperty DbFileNameProperty = DependencyProperty.RegisterAttached("DbFileName", typeof(string), typeof(ConnectionParameters));
        public static string GetDbFileName(DependencyObject obj)
        {
            return (string)obj.GetValue(DbFileNameProperty);
        }
        public static void SetDbFileName(DependencyObject obj, string value)
        {
            obj.SetValue(DbFileNameProperty, value);
            Connection._dbFileName = value;
        }


        public static readonly DependencyProperty SQLitePlatformProperty = DependencyProperty.RegisterAttached("SQLitePlatform", typeof(SQLite.Net.Interop.ISQLitePlatform), typeof(ConnectionParameters));
        public static SQLite.Net.Interop.ISQLitePlatform GetSQLitePlatform(DependencyObject obj)
        {
            return (SQLite.Net.Interop.ISQLitePlatform)obj.GetValue(SQLitePlatformProperty);
        }
        public static void SetSQLitePlatform(DependencyObject obj, SQLite.Net.Interop.ISQLitePlatform value)
        {
            obj.SetValue(SQLitePlatformProperty, value);
            Connection._sqlitePlatform = value;
        }


        public static readonly DependencyProperty EndpointConfigurationNameProperty = DependencyProperty.RegisterAttached("EndpointConfigurationName", typeof(string), typeof(ConnectionParameters));
        public static string GetEndpointConfigurationName(DependencyObject obj)
        {
            return (string)obj.GetValue(EndpointConfigurationNameProperty);
        }
        public static void SetEndpointConfigurationName(DependencyObject obj, string value)
        {
            obj.SetValue(EndpointConfigurationNameProperty, value);
            Connection._endpointConfigurationName = value;
        }


        public static readonly DependencyProperty EndpointConfigurationAddressProperty = DependencyProperty.RegisterAttached("EndpointConfigurationAddress", typeof(string), typeof(ConnectionParameters));
        public static string GetEndpointConfigurationAddress(DependencyObject obj)
        {
            return (string)obj.GetValue(EndpointConfigurationAddressProperty);
        }
        public static void SetEndpointConfigurationAddress(DependencyObject obj, string value)
        {
            obj.SetValue(EndpointConfigurationAddressProperty, value);
            Connection._endpointConfigurationAddress = value;
        }

    }
}
