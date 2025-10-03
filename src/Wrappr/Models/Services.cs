using System.ServiceProcess;

namespace Wrappr.Models;

public static class Services
{
	public static IEnumerable<ServiceController> GetAll()
	{
		return ServiceController.GetServices().OrderBy(it => it.ServiceName);
	}
}