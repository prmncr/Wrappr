namespace Wrappr.ElevatedService;

[DataContract]
public record ServiceWorkerResult {
	[DataMember] public bool IsSuccess;
	[DataMember] public string? ExceptionMessage;
}