using System.ComponentModel;
using System.Runtime.CompilerServices;
using MonitorRGBController.Annotations;

namespace MonitorRGBController;

public sealed class Test : INotifyPropertyChanged {
	private int _age;
	private string _name;

	public Test(string name, int age) {
		this.Name = name;
		this.Age = age;
	}

	public string Name {
		get => _name;
		set {
			if (value == _name) return;
			_name = value;
			OnPropertyChanged();
		}
	}

	public int Age {
		get => _age;
		set {
			if (value == _age) return;
			_age = value;
			OnPropertyChanged();
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	[NotifyPropertyChangedInvocator]
	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}