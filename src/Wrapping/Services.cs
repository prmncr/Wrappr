using System.ServiceProcess;

namespace Wrappr.Wrapping;

public static class Services {
	public static IEnumerable<ServiceController> ListAllProperty => ListAll();

	public static IEnumerable<ServiceController> ListAll() {
		return ServiceController.GetServices().ToList().OrderBy(it => it.ServiceName);
	}
}