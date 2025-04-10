using System.Text.Json.Serialization;

namespace Wrappr.Data;

public record Settings {
	[JsonInclude] public List<WrapperConfig> WrappedServices = [];
    [JsonInclude] public WindowPosition WindowPosition = new();
}