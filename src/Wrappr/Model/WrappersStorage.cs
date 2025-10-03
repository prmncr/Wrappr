using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Wrappr.Data;
using Wrappr.Model;

namespace Wrappr.Model;

public static class WrappersStorage
{
	public static ReadOnlyObservableCollection<Wrapper> Items { get; }
	public static readonly RelayCommand<Wrapper> RemoveCommand = new(Remove!);

	private static readonly ObservableCollection<Wrapper> Storage = [];

	static WrappersStorage()
	{
		Items = new ReadOnlyObservableCollection<Wrapper>(Storage);
		var services = DataStore.Settings.WrappedServices;
		foreach (var service in services)
		{
			Storage.Add(new Wrapper(new WrapperConfig(service.Name, service.Tracked, service.PollingDelay, service.Notified)));
		}
		Storage.CollectionChanged += async (_, _) => await Save();
	}

	public static async Task Save()
	{
		DataStore.Settings.WrappedServices = Items.Select(WrapperConfig.FromWrapper).ToList();
		await DataStore.Save();
	}

	public static void Add(Wrapper wrapper)
	{
		Storage.Add(wrapper);
	}

	public static void Remove(Wrapper wrapper)
	{
		Storage.Remove(wrapper);
	}

	public static WrapperSettings GetBackupFor(Wrapper wrapper)
	{
		var config = DataStore.Settings.WrappedServices
			.FirstOrDefault(it => it?.Name == wrapper.ServiceName, null);
		return config ?? throw new NullReferenceException($"Wrapper of '{wrapper.ServiceName}' was not found during backup process");
	}
}