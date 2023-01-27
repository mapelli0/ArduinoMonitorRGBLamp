using System.Windows;

namespace MonitorRGBController {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public TestViewModel ViewModel { get; set; }
		
		public MainWindow() {
			this.DataContext = new TestViewModel();
			InitializeComponent();
			
		}
	}
}