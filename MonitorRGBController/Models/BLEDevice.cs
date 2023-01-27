using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Windows.Devices.Enumeration;

namespace MonitorRGBController.Models; 

public class BLEDevice : INotifyPropertyChanged{

	public BLEDevice(DeviceInformation deviceInformation) {
		DeviceInformation = deviceInformation;
		OnPropertyChanged(nameof(DeviceInformation));
	}
	public DeviceInformation DeviceInformation { get; private init; }

	public string Id => DeviceInformation.Id;
	public string Name => DeviceInformation.Name;
	public bool IsPaired => DeviceInformation.Pairing.IsPaired;
	public bool IsConnected => (bool?)DeviceInformation.Properties["System.Devices.Aep.IsConnected"] == true;
	public bool IsConnectable => (bool?)DeviceInformation.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"] == true;

	public IReadOnlyDictionary<string, object> Properties => DeviceInformation.Properties;
	
	public event PropertyChangedEventHandler PropertyChanged;

	public void Update(DeviceInformationUpdate deviceInfoUpdate)
	{
		DeviceInformation.Update(deviceInfoUpdate);

		OnPropertyChanged("Id");
		OnPropertyChanged("Name");
		OnPropertyChanged("DeviceInformation");
		OnPropertyChanged("IsPaired");
		OnPropertyChanged("IsConnected");
		OnPropertyChanged("Properties");
		OnPropertyChanged("IsConnectable");
	}

	protected void OnPropertyChanged(string name)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}