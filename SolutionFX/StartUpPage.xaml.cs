using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using DeveloperFX.Server;
using DeveloperFX.structs;

namespace DeveloperFX
{
    /// <summary>
    /// Interaction logic for StartUpPage.xaml
    /// </summary>
    public partial class StartUpPage : Page
    {
        private MainWindow mainWindow;
        private Registry registry;
        private Backend backend;

        public StartUpPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            backend = mainWindow.backend;
            InitializeComponent();
            initializeRegistry();
        }

        private void launchClicked(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo start = new ProcessStartInfo();

            AppRegistryEntry launchConfig = registry.getRegistryEntry(configList.SelectedValue.ToString());
            start.Arguments = launchConfig.pathToIni;
            start.FileName = launchConfig.pathToExe;
            Process.Start(start);

            registry.setLastUsedConfig(configList.SelectedValue.ToString());

            mainWindow.launchClicked();
            backend.sendWebSiteCustomMessage("__launch__");
        }

        private void deleteClicked(object sender, RoutedEventArgs e)
        {
            string keyName = configList.SelectedValue.ToString();
            registry.deleteEntry(keyName);
            configList.Items.Remove(keyName);
        }

        private void configListChanged(object sender, SelectionChangedEventArgs e)
        {
            if (configList.SelectedValue == null)
            {
                configList.SelectedIndex = 0;
            }
            if (configList.SelectedValue.ToString().Length > 0)
            {
                launch.IsEnabled = true;
                deleteConfig.IsEnabled = true;
                setConfigFeilds(configList.SelectedValue.ToString());
            }
            else
            {
                launch.IsEnabled = false;
                deleteConfig.IsEnabled = false;
                removeConfigFeilds();
            }
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
            if (registry.containsKey(configName))
            {
                registry.modifyEntry(configName, pathToExe.Text, pathToIni.Text);
                confirmationLabel.Text = $"Configuration \"{configName}\" modified.";
            }
            else
            {
                registry.createEntry(configName, pathToExe.Text, pathToIni.Text);
                configList.Items.Add(configName);
                confirmationLabel.Text = $"Configuration {configName} created.";
            }
        }




        private void nameChanged(object sender, TextChangedEventArgs e)
        {
            createModify.IsEnabled = shouldCreateBeEnabled();
        }

        private bool shouldCreateBeEnabled()
        {
            if (name.Text.ToString().Length > 0 && pathToExe.Text.ToString().Length > 0 && pathToIni.Text.ToString().Length > 0)
            {
                return true;
            }
            return false;
        }
        private void removeConfigFeilds()
        {
            name.Text = "";
            pathToExe.Text = "";
            pathToIni.Text = "";
        }

        private void setConfigFeilds(string configName)
        {
            if (registry.containsKey(configName))
            {
                AppRegistryEntry entry = registry.getRegistryEntry(configName);

                name.Text = configName;
                pathToExe.Text = entry.pathToExe;
                pathToIni.Text = entry.pathToIni;
            }
        }
        private void setConfig(string configName)
        {
            configList.SelectedIndex = configList.Items.IndexOf(configName);
        }
        private void initializeRegistry()
        {
            registry = new Registry();

            foreach (string name in registry.getKeyList())
            {
                configList.Items.Add(name);
            }

            setConfig(registry.getLastUsedConfig());
        }
    }
}
