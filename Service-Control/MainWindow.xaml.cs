using System;
using System.Collections.ObjectModel;
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
using System.Diagnostics;
using System.ServiceProcess;
using System.Management;
using System.ComponentModel;

namespace Service_Control {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class ServiceInfo : INotifyPropertyChanged {
        private string _status;
        private bool _canBeStopped;
        private bool _canBeContinued;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Status { get => _status; set { _status = value; OnPropertyChanged("Status"); CanBeStopped = Status == "Running" ? true : false; CanBeContinued = Status == "Stopped" ? true : false; } }
        public string Account { get; set; }
        public bool CanBeStopped { get => _canBeStopped; set { _canBeStopped = value; OnPropertyChanged("CanBeStopped"); } }
        public bool CanBeContinued { get => _canBeContinued; set { _canBeContinued = value; OnPropertyChanged("CanBeContinued"); } }

        public ServiceInfo(ServiceController sc) {
            Name = sc.ServiceName;
            DisplayName = sc.DisplayName;
            Status = sc.Status.ToString();
            CanBeStopped = Status == "Running" ? true : false;
            CanBeContinued = Status == "Stopped" ? true : false;
        }

        private void GetAccountName(string serviceName) {
            SelectQuery query = new SelectQuery(string.Format("SELECT * FROM Win32_Service WHERE Name='{0}'", serviceName));
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query)) {
                foreach (ManagementObject s in searcher.Get()) {
                    Trace.WriteLine(s["StartName"]);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ObservableCollection<ServiceInfo> services = new ObservableCollection<ServiceInfo>(ServiceController.GetServices().Select(x => new ServiceInfo(x)));
            Trace.WriteLine(services);
            serviceInfo.DataContext = services;
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            var service = (ServiceInfo)((Button)sender).Tag;
            Trace.WriteLine(service.Name);
            ServiceController sc = new ServiceController(service.Name);
            sc.Stop();
            service.Status = "Stopped";
        }

        private void Continue(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            var service = (ServiceInfo)((Button)sender).Tag;
            Trace.WriteLine(service.Name);
            ServiceController sc = new ServiceController(service.Name);
            sc.Start();
            service.Status = "Running";
        }
    }
}
