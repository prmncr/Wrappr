using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
#if !DEBUG
using Wrappr.Resources;
#endif

namespace Wrappr.Data;

public partial class SnackbarData : ObservableObject
{
	[ObservableProperty] public partial string Title { get; set; }

	[ObservableProperty] public partial string? Message { get; set; }

	[ObservableProperty] public partial InfoBarSeverity Severity { get; set; } = InfoBarSeverity.Informational;

	public SnackbarData(string title, InfoBarSeverity severity, string? message = null)
	{
		Title = title;
		Severity = severity;
		Message = message;
	}

	private SnackbarData(Exception exception)
	{
		#if DEBUG
		Title = exception.Message;
		Message = exception.StackTrace ?? "";
		#else
		Title = Strings.ErrorMessageTitle;
		Message = exception.Message;
		#endif
		Severity = InfoBarSeverity.Error;
	}

	public static implicit operator SnackbarData(Exception exception)
	{
		return new SnackbarData(exception);
	}
}