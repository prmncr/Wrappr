using System.Text.Json.Serialization;
using Wrappr.Data;

namespace Wrappr.Utilities;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Settings))]
[JsonSerializable(typeof(WrapperConfig))]
[JsonSerializable(typeof(WindowPosition))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
public partial class SettingsHolderSerializationContext : JsonSerializerContext;