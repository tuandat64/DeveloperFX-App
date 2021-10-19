using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WebSocketSharp;

namespace DeveloperFX
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AppNotifyIcon icon;
        private Logger log;
        private void StartApp(object sender, StartupEventArgs e)
        {
            MainWindow main = new MainWindow(this);
            main.Show();

            //log = new EventLog("Log_" + DateTime.Now);
            log = new Logger(LogLevel.Fatal);
        }

        public void createNotifyIcon()
        {
            icon = new AppNotifyIcon();
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            log.File = "./Log_" + DateTime.Now;
            log.Fatal("An unexpected application exception occurred : " + args.Exception);

            MessageBox.Show("An unexpected exception has occurred. Shutting down the application. Please check the log file for more details.");

            // Prevent default unhandled exception processing
            args.Handled = true;

            Environment.Exit(0);
        }

    }
}
