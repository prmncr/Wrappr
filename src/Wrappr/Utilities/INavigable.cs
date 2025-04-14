namespace Wrappr.Utilities;

public interface INavigable {
	public string NavigationTag { get; }
	public static abstract string TypeNavigationTag { get; }

	public static abstract string NodeName(object? dataContext);
}