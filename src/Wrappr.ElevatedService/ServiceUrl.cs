using System.Text;

namespace Wrappr.ElevatedService;

public class ServiceUrl {
	public const string HostValue = "localhost";
	public const string PortValue = "54774";
	public const string PathValue = "WrapprService";
	public const string WsEndpointValue = "ws";
	public const string HttpEndpointValue = "http";

	public bool HttpProtocol { get; set; }

	public bool Host { get; set; }

	public bool Port { get; set; }

	public bool Path { get; set; }

	public bool Ws { get; set; }

	public bool Http { get; set; }

	public static implicit operator string(ServiceUrl url) {
		var builder = new StringBuilder();
		if (url.HttpProtocol) builder.Append("http://");
		if (url.Host) builder.Append(HostValue);
		if (url.Port) builder.Append(':').Append(PortValue);
		if (url.Path) builder.Append('/').Append(PathValue);
		if (url.Ws) builder.Append('/').Append(WsEndpointValue);
		if (url.Http) builder.Append('/').Append(HttpEndpointValue);
		return builder.ToString();
	}
}