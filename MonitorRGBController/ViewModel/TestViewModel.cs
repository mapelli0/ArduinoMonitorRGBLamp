using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MonitorRGBController;

public class TestViewModel {
	private ICommand _addTestCommand;


	public TestViewModel() {
		this.Tests = new ObservableCollection<Test>();
		this.Tests.Add(new Test("Initial", 10));
	}
	
	public ObservableCollection<Test> Tests { get; private set; }

	public ICommand AddTestCommand {
		get {
			return _addTestCommand ??= new CommandHandler(AddTest, true);
		}
	}

	public void AddTest() {
		var item = new Test($"Ola mundo {this.Tests.Count}", this.Tests.Count + 10);
		this.Tests.Add(item);
	}
}

public class CommandHandler: ICommand {  
	private Action _action;  
	private bool _canExecute;  
	public CommandHandler(Action action, bool canExecute) {  
		_action = action;  
		_canExecute = canExecute;  
	}  
  
	public bool CanExecute(object parameter) {  
		return _canExecute;  
	}  
  
	public event EventHandler CanExecuteChanged;  
  
	public void Execute(object parameter) {  
		_action();  
	}  
}  
