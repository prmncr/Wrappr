﻿using System.Collections.ObjectModel;
using Wrappr.Utils;

namespace Wrappr.Wrapping;

public static class Wrappers {
	static Wrappers() {
		var services = Properties.Settings.WrappedServices;
		foreach (var service in services) {
			Storage.Add(new Wrapper(new WrapperConfig(service.Name, service.Tracked, service.PollingDelay, service.Notified)));
		}
		Storage.CollectionChanged += Save;
	}

	public static ObservableCollection<Wrapper> Storage { get; } = [];

	public static void Save() {
		Properties.Settings.WrappedServices = Storage.Where(it => it.IsInitialized).Select(WrapperConfig.FromWrapper).ToList();
		Properties.Save();
	}

	private static void Save(object? arg1, object? arg2) {
		Save();
	}
}