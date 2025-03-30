using System.Text.Json;
using Wrappr.Data;
using Wrappr.Utilities;

namespace Wrappr.Services;

public static class Properties {
	private const string SettingsFileName = "wrappr-config.json";
	private static readonly string SettingsPath = AppDomain.CurrentDomain.BaseDirectory + SettingsFileName;
	private static Settings? _settings;

	public static Settings Settings
	{
		get {
			if (_settings != null) return _settings;

			if (File.Exists(SettingsPath)) {
				try {
					_settings = JsonSerializer.Deserialize(File.ReadAllText(SettingsPath), SettingsHolderSerializationContext.Default.Settings) ?? new Settings();
				} catch (JsonException) {
					_settings = new Settings();
				}
			} else {
				_settings = new Settings();
			}
			return _settings;
		}
	}

	public static void Save() {
		File.WriteAllText(SettingsPath, JsonSerializer.Serialize(_settings, SettingsHolderSerializationContext.Default.Settings!));
	}
}