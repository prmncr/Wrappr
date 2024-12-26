using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wrappr.Wrapping;

namespace Wrappr.Utils;

public static class Properties {
	private const string SettingsFileName = "wrappr-config.json";
	private static SettingsHolder? _settings;

	public static SettingsHolder Settings
	{
		get {
			if (_settings != null) return _settings;

			if (File.Exists(SettingsFileName)) {
				try {
					_settings = JsonSerializer.Deserialize<SettingsHolder>(File.ReadAllText(SettingsFileName)) ?? new SettingsHolder();
				} catch (JsonException) {
					_settings = new SettingsHolder();
				}
			} else {
				_settings = new SettingsHolder();
			}
			return _settings;
		}
	}

	public static void Save() {
		File.WriteAllText(SettingsFileName, JsonSerializer.Serialize(_settings));
	}

	public class SettingsHolder {
		[JsonInclude] public List<WrapperConfig> WrappedServices = [];
	}
}