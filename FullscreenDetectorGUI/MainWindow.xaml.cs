using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace FullscreenDetectorGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event EventHandler SettingsSaved;

        public List<string> processNames = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            ReloadSettings();
        }

        public void ReloadSettings()
        {
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                if (!processNames.Contains(process.ProcessName))
                {
                    processNames.Add(process.ProcessName);
                }
            }

            processNames.Sort();

            comboProcesses.ItemsSource = processNames;

            string processName = Properties.Settings.Default.ProcessName;
            comboProcesses.SelectedItem = processName;
            labelCurrent.Content = processName.Equals("") ? "None" : processName;

            cbAutostart.IsChecked = Properties.Settings.Default.Autostart;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        protected override void OnClosed(EventArgs e)
        {
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            string processname = comboProcesses.SelectedValue == null ? "" : comboProcesses.SelectedValue.ToString();
            Properties.Settings.Default.ProcessName = processname;
            Properties.Settings.Default.Autostart = cbAutostart.IsChecked.Value;
            Properties.Settings.Default.Save();

            if (SettingsSaved != null)
            {
                SettingsSaved(this, EventArgs.Empty);
            }

            this.Hide();
        }
    }
}