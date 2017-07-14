using Microsoft.Win32;
using System;
using System.Windows;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Management;

namespace ServiceInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string SERVICE_NAME_UNDEFINED = "Service name: undefined";
        private OpenFileDialog openFileDialog = new OpenFileDialog();
        private string serviceName;

        private string GetServiceName(string fileNameService)
        {
            string result = null;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service");
            ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject obj in collection)
            {
                string name = obj["Name"] as string;
                string pathName = obj["PathName"] as string;
                if (pathName != null && pathName.Contains(fileNameService))
                {
                    result = name;
                    break;
                }
            }
            tbServiceName.Text = (result == null) ? SERVICE_NAME_UNDEFINED : string.Format("Service name: {0}", result);

            return result;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Set filter for file extension and default file extension
            openFileDialog.DefaultExt = ".exe";
            openFileDialog.Filter = "NT-Services (.exe)|*.exe";

            if (openFileDialog.ShowDialog() == true)
            {
                tbFileServiceName.Text = openFileDialog.SafeFileName;
                serviceName = GetServiceName(openFileDialog.FileName);
            }
            else
            {
                tbFileServiceName.Text = "";
                serviceName = "";
            }
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            if (tbFileServiceName.Text.Length == 0)
            {
                MessageBox.Show("Select file NT-Service!");
                return;
            }

            try
            {
                ManagedInstallerClass.InstallHelper(new[] { openFileDialog.FileName });
                serviceName = GetServiceName(openFileDialog.FileName);
                MessageBox.Show(string.Format("Service \"{0}\" has been installed!", openFileDialog.SafeFileName));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void btnUninstall_Click(object sender, RoutedEventArgs e)
        {
            if (tbFileServiceName.Text.Length == 0)
            {
                MessageBox.Show("Select file NT-Service!");
                return;
            }

            try
            {
                ManagedInstallerClass.InstallHelper(new[] { @"/u", openFileDialog.FileName });
                serviceName = GetServiceName(openFileDialog.FileName);
                MessageBox.Show(string.Format("Service \"{0}\" has been uninstalled!", openFileDialog.SafeFileName));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ServiceController controller;
            try
            {
                controller = new ServiceController { ServiceName = serviceName };
                controller.Start();
                MessageBox.Show(string.Format("Service \"{0}\" Started", serviceName));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            ServiceController controller;
            try
            {
                controller = new ServiceController { ServiceName = serviceName };
                controller.Stop();
                MessageBox.Show(string.Format("Service \"{0}\" Stopped", serviceName));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }
    }
}
