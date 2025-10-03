using System.Text.Json.Serialization;
using Wrappr.Models;
using Wrapper = Wrappr.ViewModels.Wrapper;

namespace Wrappr.Data;

public record WrapperConfig
{
	[JsonInclude] public string? Name { get; set; }

	[JsonInclude] public bool Tracked { get; set; }

	[JsonInclude] public bool Notified { get; set; }

	[JsonInclude] public int PollingDelay { get; set; }

	public WrapperConfig(string? name = null, bool tracked = true, int pollingDelay = 1000, bool notified = true)
	{
		if (name != null)
		{
			Name = name;
		}
		Tracked = tracked;
		Notified = notified;
		PollingDelay = pollingDelay;
	}

	public static implicit operator WrapperSettings(WrapperConfig config)
	{
		return new WrapperSettings
		{
			Tracked = config.Tracked,
			Notified = config.Notified,
			PollingDelay = config.PollingDelay
		};
	}

	public static implicit operator WrapperConfig(WrapperSettings settings)
	{
		return new WrapperConfig
		{
			Tracked = settings.Tracked,
			Notified = settings.Notified,
			PollingDelay = settings.PollingDelay
		};
	}

	public static WrapperConfig FromWrapper(Wrapper wrapper)
	{
		return new WrapperConfig
		{
			Name = wrapper.ServiceName,
			Tracked = wrapper.Settings.Tracked,
			PollingDelay = wrapper.Settings.PollingDelay,
			Notified = wrapper.Settings.Notified
		};
	}
}