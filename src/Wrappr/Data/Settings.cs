using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Wrappr.Data;

public partial class Settings : ObservableObject
{
	[JsonInclude] public List<WrapperConfig> WrappedServices = [];
	[JsonInclude] [ObservableProperty] public partial bool TrayMenuWrappersInTop { get; set; } = true;
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Settings))]
[JsonSerializable(typeof(WrapperConfig))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
public partial class SettingsSerializationContext : JsonSerializerContext;