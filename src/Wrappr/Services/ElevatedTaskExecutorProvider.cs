using System.Runtime.InteropServices;
using Wrappr.ElevatedService;
using Wrappr.Executor;
using ServiceController = System.ServiceProcess.ServiceController;

namespace Wrappr.Services;

public static class ElevatedTaskExecutorProvider {
	private static ElevatedServiceWorkerClient _client = null!;

	public static void Connect() {
		_client = new ElevatedServiceWorkerClient(
			ElevatedServiceWorkerClient.EndpointConfiguration.BasicHttpBinding_IElevatedServiceWorker,
			new ServiceUrl {
				HttpProtocol = true,
				Host = true,
				Port = true,
				Path = true,
				Http = true
			}
		);
	}

	public static async Task Start(ServiceController service) {
		var result = await _client.StartAsync(service.ServiceName);
		if (!result.IsSuccess) throw new ExternalException($"Error occured in Wrappr Service\n{result.ExceptionMessage}");
	}

	public static async Task Stop(ServiceController service) {
		var result = await _client.StopAsync(service.ServiceName);
		if (!result.IsSuccess) throw new ExternalException($"Error occured in Wrappr Service\n{result.ExceptionMessage}");
	}
}