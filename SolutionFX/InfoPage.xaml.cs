using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using DeveloperFX.Server;

namespace DeveloperFX
{
    /// <summary>
    /// Interaction logic for InfoPanel.xaml
    /// </summary>
    public partial class InfoPage : Page
    {
        private MainWindow mainWindow;
        private bool isConnected = false;


        public InfoPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();

            isConnectedText.Text = isConnected.ToString();
        }

        public void connected()
        {
            isConnected = true;
            Dispatcher.BeginInvoke(
                new ThreadStart(() => isConnectedText.Text = isConnected.ToString()));
            
        }

        public void setMessageText(string msg)
        {
            Dispatcher.BeginInvoke(
                new ThreadStart(() => mainWindow.infoPage.message.Text = msg));
        }
    }
}
