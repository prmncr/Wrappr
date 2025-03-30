using System.Text.Json.Serialization;

namespace Wrappr.Data;

public class Settings {
	[JsonInclude] public List<WrapperConfig> WrappedServices = [];
}