using System.Text.Json.Serialization;
using Wrappr.Model;

namespace Wrappr.Data;

public record WrapperConfig {
	[JsonInclude] public string? Name { get; init; }

	[JsonInclude] public bool Tracked { get; init; }

	[JsonInclude] public bool Notified { get; init; }

	[JsonInclude] public int PollingDelay { get; init; }

	public WrapperConfig(string? name = null, bool tracked = false, int pollingDelay = 1000, bool notified = false) {
		if (name != null) {
			Name = name;
		}
		Tracked = tracked;
		Notified = notified;
		PollingDelay = pollingDelay;
	}

	public static WrapperConfig FromWrapper(Wrapper wrapper) {
		return new WrapperConfig {
			Name = wrapper.ServiceName,
			Tracked = wrapper.IsTrackingEnabled,
			PollingDelay = wrapper.PollingDelay,
			Notified = wrapper.IsNotificationsEnabled
		};
	}
}