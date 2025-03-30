namespace Wrappr.Utilities;

public static class Arguments {
	private static readonly Dictionary<string, object?> Args = [];

	private static bool GetBoolean(string name) {
		if (!Args.TryGetValue(name, out var value)) return false;
		if (value is bool b)
			return b;
		return false;
	}

	private static string? GetString(string name) {
		if (!Args.TryGetValue(name, out var value)) return null;
		if (value is string s)
			return s;
		return null;
	}

	public static void Initialize(string[] args) {
		for (var i = 0; i < args.Length; i++) {
			if (!args[i].StartsWith('-')) continue;
			if (i + 1 == args.Length || args[i + 1].StartsWith('-')) {
				Args.TryAdd(args[i], true);
				continue;
			}
			Args.TryAdd(args[i], args[++i]);
		}
	}

	public static class Boot {
		public static bool IsSilentMode => GetBoolean("--silent");
	}
}