using System.ServiceProcess;

namespace Wrappr.ElevatedService;

internal class Executor : IElevatedServiceWorker {
	public ServiceWorkerResult Start(string serviceName) {
		try {
			ServiceController.GetServices().FirstOrDefault(it => it.ServiceName == serviceName)?.Start();
			return new ServiceWorkerResult { IsSuccess = true };
		} catch (Exception e) {
			return new ServiceWorkerResult { IsSuccess = false, ExceptionMessage = e.Message };
		}
	}

	public ServiceWorkerResult Stop(string serviceName) {
		try {
			ServiceController.GetServices().FirstOrDefault(it => it.ServiceName == serviceName)?.Stop();
			return new ServiceWorkerResult { IsSuccess = true };
		} catch (Exception e) {
			return new ServiceWorkerResult { IsSuccess = false, ExceptionMessage = e.Message };
		}
	}
}