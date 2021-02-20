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
using Microsoft.Win32;
using System.Diagnostics;

namespace solutionfx_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RegistryKey appKey;
        RegistryKey configKey;
        Dictionary<string, RegistryEntry> configMap;

        public MainWindow()
        {
            InitializeComponent();

            appKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32).OpenSubKey("Software\\SolutionFX");
            configKey = appKey.OpenSubKey("Configurations", true);

            Console.WriteLine("Test");
            configMap = getExistingConfigs();

            foreach(string name in configMap.Keys.ToList())
            {
                configList.Items.Add(name);
            }
        }

        private void configListChanged(object Sender, RoutedEventArgs e)
        {
            if(configList.SelectedValue.ToString().Length == 0)
            {
                launch.IsEnabled = false;
            } else
            {
                launch.IsEnabled = true;
            }
        }

        private Dictionary<string, RegistryEntry> getExistingConfigs()
        {
            Dictionary<string, RegistryEntry> map = new Dictionary<string, RegistryEntry>();

            string[] names = configKey.GetSubKeyNames();
            if (names.Length > 0)
            {
                foreach(string name in names)
                {
                    RegistryKey currKey = configKey.OpenSubKey(name);

                    RegistryEntry newEntry = new RegistryEntry();
                    newEntry.name = name;
                    newEntry.pathToExe = currKey.GetValue("path_to_exe").ToString();
                    newEntry.pathToIni = currKey.GetValue("path_to_ini").ToString();

                    map.Add(name, newEntry);
                }
            }
            else
            {

            }
            return map;
        }

        private void configListChanged(object sender, SelectionChangedEventArgs e)
        {
            if(configList.SelectedValue == null)
            {
                configList.SelectedIndex = 0;
            }
            if(configList.SelectedValue.ToString().Length > 0)
            {
                launch.IsEnabled = true;
                deleteConfig.IsEnabled = true;
            } 
            else
            {
                launch.IsEnabled = false;
                deleteConfig.IsEnabled = false;
            }
        }

        private bool shouldCreateBeEnabled() { 
            if(name.Text.ToString().Length > 0 && pathToExe.Text.ToString().Length > 0 && pathToIni.Text.ToString().Length > 0)
            {
                return true;
            }
            return false;
        }
        private void launchClicked(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo start = new ProcessStartInfo();

            RegistryEntry launchConfig = configMap[configList.SelectedValue.ToString()];
            start.Arguments = launchConfig.pathToIni;
            start.FileName = launchConfig.pathToExe;
            int exitCode;
            Process.Start(start);

            Application.Current.Shutdown();
        }

        private void deleteClicked(object sender, RoutedEventArgs e)
        {
            string keyName = configList.SelectedValue.ToString();
            configKey.DeleteSubKeyTree(keyName);
            configList.Items.Remove(keyName);
        }

        private void nameChanged(object sender, TextChangedEventArgs e)
        {
            createModify.IsEnabled = shouldCreateBeEnabled();
        }

        private void exeJoin(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".exe";

            dlg.Filter = "Executable File (terminal.exe)|terminal.exe";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                pathToExe.Text = filename;

                createModify.IsEnabled = shouldCreateBeEnabled();
            }
        }

        private void iniJoin(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".ini";

            dlg.Filter = "Init File (*.ini)|*.ini";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                pathToIni.Text = filename;


                createModify.IsEnabled = shouldCreateBeEnabled();
            }
        }

        private void createModifyClicked(object sender, RoutedEventArgs e)
        {
            string configName = name.Text.ToString();
            if (configMap.ContainsKey(configName))
            {
                configMap[configName].pathToExe = pathToExe.Text.ToString();
                configMap[configName].pathToIni = pathToIni.Text.ToString();
                modifyKey(configMap[configName]);
            }
            else
            {
                RegistryEntry newEntry = new RegistryEntry();
                newEntry.name = configName;
                newEntry.pathToExe = pathToExe.Text.ToString();
                newEntry.pathToIni = pathToIni.Text.ToString();
                addKey(newEntry);
                configMap.Add(configName, newEntry);
                configList.Items.Add(configName);
            }
        }

        private void addKey(RegistryEntry newEntry)
        {
            configKey.CreateSubKey(newEntry.name);
            RegistryKey currentConfigKey = configKey.OpenSubKey(newEntry.name, true);
            currentConfigKey.SetValue("path_to_exe", newEntry.pathToExe);
            currentConfigKey.SetValue("path_to_ini", newEntry.pathToIni);
        }

        private void modifyKey(RegistryEntry newEntry)
        {
            RegistryKey currentConfigKey = configKey.OpenSubKey(newEntry.name, true);
            currentConfigKey.SetValue("path_to_exe", newEntry.pathToExe);
            currentConfigKey.SetValue("path_to_ini", newEntry.pathToIni);
        }
    }

    public class RegistryEntry
    {
        public string name;
        public string pathToExe;
        public string pathToIni;
    } 
}
