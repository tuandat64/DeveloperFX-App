using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Drawing;
using Microsoft.Win32;
using DeveloperFX.Server;
using DeveloperFX.structs;


namespace DeveloperFX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private App app;
        public Backend backend { get; }

        public InfoPage infoPage { get; }
        public StartUpPage startUpPage { get; }
        public bool isLaunched { get; set; }

        public MainWindow(App app)
        {
            this.app = app;
            backend = new Backend(this);
            InitializeComponent();

            startUpPage = new StartUpPage(this);
            infoPage = new InfoPage(this);

            SizeToContent = SizeToContent.WidthAndHeight;
            Content = startUpPage;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            isLaunched = false;
        }

        private void onClose(object sender, CancelEventArgs e)
        {
            backend.sendWebSiteCustomMessage("__exit__");
            backend.close();
        }

        public void launchClicked()
        {
            Content = infoPage;
            backend.launchSocketServer();
            isLaunched = true;
        }
    }
}
