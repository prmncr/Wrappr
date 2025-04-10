namespace Wrappr.ElevatedService;

[ServiceContract]
public interface IElevatedServiceWorker {
	[OperationContract]
	public ServiceWorkerResult Start(string serviceName);

	[OperationContract]
	public ServiceWorkerResult Stop(string serviceName);
}