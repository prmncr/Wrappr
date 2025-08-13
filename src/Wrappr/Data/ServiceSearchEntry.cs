using Wrappr.Resources;

namespace Wrappr.Data;

public record ServiceSearchEntry
{
	public string ServiceName { get; init; } = Strings.EmptyWrapperServiceName;

	public string DisplayName { get; init; } = Strings.EmptyWrapperDisplayName;

	public bool IsPlaceholder { get; init; }
}