using System.Collections.ObjectModel;
using Wrappr.Data;
using Wrappr.Services;

namespace Wrappr.Model;

public class Wrappers
{
	public static Wrappers Instance { get; private set; } = null!;

	public ObservableCollection<Wrapper> Storage { get; } = [];

	public static void Initialize()
	{
		Instance = new Wrappers();
		var services = Properties.Settings.WrappedServices;
		foreach (var service in services)
		{
			Instance.Storage.Add(new Wrapper(new WrapperConfig(service.Name, service.Tracked, service.PollingDelay, service.Notified)));
		}
		Instance.Storage.CollectionChanged += (_, _) => Instance.Save();
	}

	public void Save()
	{
		Properties.Settings.WrappedServices = Storage.Where(it => it.IsInitialized).Select(WrapperConfig.FromWrapper).ToList();
		Properties.Save();
	}
}