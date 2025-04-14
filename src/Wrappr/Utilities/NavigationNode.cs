namespace Wrappr.Utilities;

public class NavigationNode(string tag, string nodeName, object? dataContext) {
	public readonly string NodeName = nodeName;
	public readonly string Tag = tag;
	public readonly object? DataContext = dataContext;
}