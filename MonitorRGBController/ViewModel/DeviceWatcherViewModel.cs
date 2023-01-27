using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using MonitorRGBController.Models;

namespace MonitorRGBController;

public class DeviceWatcherViewModel {
	public ObservableCollection<BLEDevice> KnownDevices;
	private DeviceWatcher deviceWatcher;
	private List<DeviceInformation> UnknownDevices = new List<DeviceInformation>();

	public DeviceWatcherViewModel() {
		KnownDevices = new ObservableCollection<BLEDevice>();
	}

	/// <summary>
	/// Starts a device watcher that looks for all nearby Bluetooth devices (paired or unpaired). 
	/// Attaches event handlers to populate the device collection.
	/// </summary>
	public void StartBleDeviceWatcher() {
		// Additional properties we would like about the device.
		// Property strings are documented here https://msdn.microsoft.com/en-us/library/windows/desktop/ff521659(v=vs.85).aspx
		string[] requestedProperties = {
										   "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable"
									   };

		// BT_Code: Example showing paired and non-paired in a single query.
		string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

		deviceWatcher =
			DeviceInformation.CreateWatcher(
											aqsAllBluetoothLEDevices,
											requestedProperties,
											DeviceInformationKind.AssociationEndpoint);

		// Register event handlers before starting the watcher.
		deviceWatcher.Added += DeviceWatcher_Added;
		deviceWatcher.Updated += DeviceWatcher_Updated;
		// deviceWatcher.Removed += DeviceWatcher_Removed;
		// deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
		// deviceWatcher.Stopped += DeviceWatcher_Stopped;

		// Start over with an empty collection.
		KnownDevices.Clear();

		// Start the watcher. Active enumeration is limited to approximately 30 seconds.
		// This limits power usage and reduces interference with other Bluetooth activities.
		// To monitor for the presence of Bluetooth LE devices for an extended period,
		// use the BluetoothLEAdvertisementWatcher runtime class. See the BluetoothAdvertisement
		// sample for an example.
		deviceWatcher.Start();
	}

	private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo) {
		// We must update the collection on the UI thread because the collection is databound to a UI element.
		Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
						  () => {
							  lock (this) {
								  Debug.WriteLine(String.Format("Added {0}{1}", deviceInfo.Id, deviceInfo.Name));

								  // Protect against race condition if the task runs after the app stopped the deviceWatcher.
								  if (sender == deviceWatcher) {
									  // Make sure device isn't already present in the list.
									  if (FindBluetoothLEDeviceDisplay(deviceInfo.Id) == null) {
										  if (deviceInfo.Name != string.Empty) {
											  // If device has a friendly name display it immediately.
											  KnownDevices.Add(new BLEDevice(deviceInfo));
										  } else {
											  // Add it to a list in case the name gets updated later. 
											  UnknownDevices.Add(deviceInfo);
										  }
									  }
								  }
							  }
						  });
	}

	private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate) {
		// We must update the collection on the UI thread because the collection is databound to a UI element.
		Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
											() => {
												lock (this) {
													Debug.WriteLine(String.Format("Updated {0}{1}", deviceInfoUpdate.Id, ""));

													// Protect against race condition if the task runs after the app stopped the deviceWatcher.
													if (sender == deviceWatcher) {
														BLEDevice bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);

														if (bleDeviceDisplay != null) {
															// Device is already being displayed - update UX.
															bleDeviceDisplay.Update(deviceInfoUpdate);
															return;
														}

														DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);

														if (deviceInfo != null) {
															deviceInfo.Update(deviceInfoUpdate);

															// If device has been updated with a friendly name it's no longer unknown.
															if (deviceInfo.Name != String.Empty) {
																KnownDevices.Add(new BLEDevice(deviceInfo));
																UnknownDevices.Remove(deviceInfo);
															}
														}
													}
												}
											});
	}
	
	
	private BLEDevice FindBluetoothLEDeviceDisplay(string id)
	{
		foreach (BLEDevice bleDeviceDisplay in KnownDevices)
		{
			if (bleDeviceDisplay.Id == id)
			{
				return bleDeviceDisplay;
			}
		}
		return null;
	}
	
	private DeviceInformation FindUnknownDevices(string id)
	{
		foreach (DeviceInformation bleDeviceInfo in UnknownDevices)
		{
			if (bleDeviceInfo.Id == id)
			{
				return bleDeviceInfo;
			}
		}
		return null;
	}
	
}