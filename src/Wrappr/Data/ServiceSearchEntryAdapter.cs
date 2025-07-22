namespace Wrappr.Data;

public record ServiceSearchEntryAdapter
{
	public string ServiceName { get; init; } = "";

	public string DisplayName { get; init; } = "";

	public bool IsPlaceholder { get; init; }
}