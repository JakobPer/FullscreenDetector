using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Media;
using Application = System.Windows.Application;

namespace FullscreenDetectorGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string AppName = "FullscreenDetector";

        private NotifyIcon nIcon;
        private MainWindow mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            mainWindow = new MainWindow();

            nIcon = new NotifyIcon();
            nIcon.Icon = FullscreenDetectorGUI.Properties.Resources.FullscreenIcon;
            nIcon.Visible = true;

            List<MenuItem> menuItems = new List<MenuItem>();
            menuItems.Add(new MenuItem("Settings", (sender, args) =>
            {
                if (!mainWindow.IsVisible)
                {
                    mainWindow.ReloadSettings();
                    mainWindow.ShowDialog();
                }
            }));
            menuItems.Add(new MenuItem("Exit", (sender, args) => Shutdown()));

            var context = new ContextMenu(menuItems.ToArray());
            nIcon.ContextMenu = context;

            mainWindow.SettingsSaved += OnMainWindowOnSettingsSaved;
            FullscreenDetector.FullscreenDetector.processName =
                FullscreenDetectorGUI.Properties.Settings.Default.ProcessName;

            Automation.AddAutomationFocusChangedEventHandler(
                FullscreenDetector.FullscreenDetector.OnFocusChangedHandler);
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Tick += (sender, args) => { FullscreenDetector.FullscreenDetector.CheckFullscreenApp(); };
            timer.Start();

            if (FullscreenDetectorGUI.Properties.Settings.Default.ProcessName.Equals(""))
            {
                mainWindow.ReloadSettings();
                mainWindow.ShowDialog();
            }
        }

        private void OnMainWindowOnSettingsSaved(object sender, EventArgs args)
        {
            FullscreenDetector.FullscreenDetector.processName =
                FullscreenDetectorGUI.Properties.Settings.Default.ProcessName;

            bool autostart = FullscreenDetectorGUI.Properties.Settings.Default.Autostart;

            if (autostart)
            {
                string path = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                AutostartUtils.addAutoStartRegistry(AppName, path);
            }
            else
            {
                AutostartUtils.removeAutostartRegistry(AppName);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            nIcon.Visible = false;

            base.OnExit(e);
        }
    }
}