namespace Wrappr.ElevatedService;

public static class Program {
	public static void Main(string[] args) {
		var builder = WebApplication.CreateBuilder(args);

		builder.Services
			.AddServiceModelServices()
			.AddServiceModelMetadata()
			.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

		var app = builder.Build();

		app.UseServiceModel(serviceBuilder =>
		{
			var binding = new WSHttpBinding(SecurityMode.None);

			serviceBuilder.AddService<Executor>();
			serviceBuilder.AddServiceEndpoint<Executor, IElevatedServiceWorker>(
				new BasicHttpBinding(),
				new ServiceUrl {
					Path = true,
					Http = true
				});
			serviceBuilder.AddServiceEndpoint<Executor, IElevatedServiceWorker>(
				binding,
				new ServiceUrl {
					Path = true,
					Ws = true
				});
		});

		var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
		serviceMetadataBehavior.HttpGetEnabled = true;

		app.Run();
	}
}