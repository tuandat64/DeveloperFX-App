using System;
using System.Drawing;
using System.Windows;

namespace DeveloperFX
{
    public class AppNotifyIcon
    {
        System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
        System.Windows.Forms.MenuItem menuItemExit = new System.Windows.Forms.MenuItem();

        public AppNotifyIcon()
        {
            notifyIcon.Icon = new Icon("SolutionFX.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "SolutionFX";
            notifyIcon.MouseClick += (s, e) => { this.iconClicked(); };
            contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.menuItemExit });
            menuItemExit.Index = 0;
            menuItemExit.Text = "E&xit";
            menuItemExit.Click += new EventHandler(this.menuItemExit_Click);
            notifyIcon.ContextMenu = this.contextMenu;
            setNotifyIconLaunched();
        }

        private void setNotifyIconLaunched()
        {
            notifyIcon.BalloonTipTitle = "MetaTrader launched";
            notifyIcon.BalloonTipText = "Socket server running in background.";
            notifyIcon.ShowBalloonTip(5);
        }

        private void menuItemExit_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            Application.Current.MainWindow.Close();
        }


        protected void iconClicked()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
            {
                Console.Out.WriteLine("Minimized");
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        }
    }
}