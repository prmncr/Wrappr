using System.ServiceProcess;

namespace Wrappr.Model;

public static class Services
{
	public static IEnumerable<ServiceController> GetAll()
	{
		return ServiceController.GetServices().OrderBy(it => it.ServiceName);
	}
}