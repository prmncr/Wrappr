using System.Text.Json;
using Wrappr.Data;

namespace Wrappr.Model;

public static class DataStore
{
	private const string SettingsFileName = "wrappr-config.json";
	private static readonly string SettingsPath = AppDomain.CurrentDomain.BaseDirectory + SettingsFileName;
	private static Settings? _settings;

	public static Settings Settings
	{
		get
		{
			if (_settings != null) return _settings;

			if (File.Exists(SettingsPath))
			{
				try
				{
					_settings = JsonSerializer.Deserialize(File.ReadAllText(SettingsPath), SettingsSerializationContext.Default.Settings) ?? new Settings();
				} catch (JsonException)
				{
					_settings = new Settings();
				}
			} else
			{
				_settings = new Settings();
			}
			return _settings;
		}
	}

	public static async Task Save()
	{
		await File.WriteAllTextAsync(SettingsPath, JsonSerializer.Serialize(_settings, SettingsSerializationContext.Default.Settings!));
	}
}