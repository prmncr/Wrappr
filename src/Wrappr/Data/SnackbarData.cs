using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Resources;

namespace Wrappr.Data;

public partial class SnackbarData : ObservableObject {
	public SnackbarData(string title, string? message, InfoBarSeverity severity) {
		Title = title;
		Severity = severity;
		Message = message;
	}

	public SnackbarData(Exception exception) {
		#if DEBUG
		Title = exception.Message;
		Message = exception.StackTrace ?? "";
		#else
		Title = Strings.ErrorMessageTitle;
		Message = exception.Message;
		#endif
		Severity = InfoBarSeverity.Error;
	}

	[ObservableProperty] public partial string Title { get; set; }

	[ObservableProperty] public partial string? Message { get; set; }

	[ObservableProperty] public partial InfoBarSeverity Severity { get; set; } = InfoBarSeverity.Informational;
}