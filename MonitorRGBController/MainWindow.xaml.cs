using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.Devices.Enumeration;
using MonitorRGBController.Models;

namespace MonitorRGBController {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private ObservableCollection<BLEDevice> KnownDevices = new ObservableCollection<BLEDevice>();
		private List<DeviceInformation> UnknownDevices = new List<DeviceInformation>();

		private DeviceWatcher deviceWatcher;
		
		
		public MainWindow() {
			InitializeComponent();
		}
		private void ListBLEDvices_Click(object sender, RoutedEventArgs e) {
			if (this.deviceWatcher == null) {
				StartBleDeviceWatcher();
				btnAdd.Content = "Stop enumerating";
			} else {
				StopBleDeviceWatcher();
				btnAdd.Content = "Start enumerating";
			}
		}
	}
}