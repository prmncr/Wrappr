using Wrappr.Utils;

namespace Wrappr;

public static class Program {
	[STAThread]
	public static void Main(string[] args) {
		Arguments.Initialize(args);
		App.Main();
	}
}