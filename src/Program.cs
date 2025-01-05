using Wrappr.Utils;
using Wrappr.Wrapping;

namespace Wrappr;

public static class Program {
	[STAThread]
	public static void Main(string[] args) {
		Arguments.Initialize(args);
		Wrappers.Initialize();
		App.Main();
	}
}