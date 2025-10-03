using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Wrappr.Data;
using Wrappr.Models;

namespace Wrappr.Models;

public static class WrappersStorage
{
	public static ReadOnlyObservableCollection<ViewModels.Wrapper> Items { get; }
	public static readonly RelayCommand<ViewModels.Wrapper> RemoveCommand = new(Remove!);

	private static readonly ObservableCollection<ViewModels.Wrapper> Storage = [];

	static WrappersStorage()
	{
		Items = new ReadOnlyObservableCollection<ViewModels.Wrapper>(Storage);
		var services = DataStore.Settings.WrappedServices;
		foreach (var service in services)
		{
			Storage.Add(new ViewModels.Wrapper(new WrapperConfig(service.Name, service.Tracked, service.PollingDelay, service.Notified)));
		}
		Storage.CollectionChanged += async (_, _) => await Save();
	}

	public static async Task Save()
	{
		DataStore.Settings.WrappedServices = Items.Select(WrapperConfig.FromWrapper).ToList();
		await DataStore.Save();
	}

	public static void Add(ViewModels.Wrapper wrapper)
	{
		Storage.Add(wrapper);
	}

	public static void Remove(ViewModels.Wrapper wrapper)
	{
		Storage.Remove(wrapper);
	}

	public static WrapperSettings GetBackupFor(ViewModels.Wrapper wrapper)
	{
		var config = DataStore.Settings.WrappedServices
			.FirstOrDefault(it => it?.Name == wrapper.ServiceName, null);
		return config ?? throw new NullReferenceException($"Wrapper of '{wrapper.ServiceName}' was not found during backup process");
	}
}