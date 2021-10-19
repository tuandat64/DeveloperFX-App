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
using System.Drawing;

namespace SolutionFX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WSS server;
        RegistryKey appKey;
        RegistryKey configKey;
        Dictionary<string, RegistryEntry> configMap;

        System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
        System.Windows.Forms.MenuItem menuItemExit = new System.Windows.Forms.MenuItem();


        public MainWindow()
        {
           
            InitializeComponent();

            this.server = Application.Current.

            this.notifyIcon.Icon = new Icon(@"./../../SolutionFX.ico");
            this.notifyIcon.Visible = true;
            this.notifyIcon.Text = "SolutionFX";
            this.notifyIcon.MouseClick += (s, e) => { this.iconClicked(); };
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.menuItemExit});
            this.menuItemExit.Index = 0;
            this.menuItemExit.Text = "E&xit";
            this.menuItemExit.Click += new EventHandler(this.menuItemExit_Click);
            this.notifyIcon.ContextMenu = this.contextMenu;
            

            appKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32).OpenSubKey("Software\\SolutionFX", true);
            configKey = appKey.OpenSubKey("Configurations", true);

            configMap = getExistingConfigs();

            foreach(string name in configMap.Keys.ToList())
            {
                configList.Items.Add(name);
            }

            string lastUsedConfig = getLastUsedConfig();
            setConfig(lastUsedConfig);
            setConfigFeilds(lastUsedConfig);

        }

        private void setNotifyIconLaunched()
        {
            this.notifyIcon.BalloonTipTitle = "MetaTrader launched";
            this.notifyIcon.BalloonTipText = "Socket server running in background.";
            this.notifyIcon.ShowBalloonTip(5);
        }

        private void launchClicked(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo start = new ProcessStartInfo();

            RegistryEntry launchConfig = configMap[configList.SelectedValue.ToString()];
            start.Arguments = launchConfig.pathToIni;
            start.FileName = launchConfig.pathToExe;
            int exitCode;
            Process.Start(start);

            setLastUsedRegestry(configList.SelectedValue.ToString());

            this.server.sendCustomMessage("__launch__");

            Hide();

            setNotifyIconLaunched();
           
        }

        private void menuItemExit_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            this.Close();
        }

        protected void iconClicked()
        {
            if (this.WindowState == WindowState.Minimized)
            {
                Console.Out.WriteLine("Minimized");
                this.WindowState = WindowState.Normal;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.server.sendCustomMessage("__exit__");
        }

        private void setConfig(string configName)
        {
            configList.SelectedIndex = configList.Items.IndexOf(configName);
        }

        private string getLastUsedConfig()
        {
            return appKey.GetValue("LastUsedConfig").ToString();
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
                setConfigFeilds(configList.SelectedValue.ToString());
            } 
            else
            {
                launch.IsEnabled = false;
                deleteConfig.IsEnabled = false;
                removeConfigFeilds();
            }
        }

        private void setConfigFeilds(string configName)
        {
            if(configMap.ContainsKey(configName))
            {
                name.Text = configName;
                pathToExe.Text = configMap[configName].pathToExe;
                pathToIni.Text = configMap[configName].pathToIni;
            }
        }

        private void removeConfigFeilds()
        {
            name.Text = "";
            pathToExe.Text = "";
            pathToIni.Text = "";
        }

        private bool shouldCreateBeEnabled() { 
            if(name.Text.ToString().Length > 0 && pathToExe.Text.ToString().Length > 0 && pathToIni.Text.ToString().Length > 0)
            {
                return true;
            }
            return false;
        }

        private void setLastUsedRegestry(string val)
        {
            appKey.SetValue("LastUsedConfig", val);
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
                confirmationLabel.Text = $"Configuration \"{configName}\" modified.";
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
                confirmationLabel.Text = $"Configuration {configName} created.";
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
