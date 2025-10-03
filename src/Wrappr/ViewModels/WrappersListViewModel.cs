using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Wrappr.Models;
using Wrappr.Views;

namespace Wrappr.ViewModels;

public static class WrappersListViewModel
{
	[field:MaybeNull, AllowNull]
	public static RelayCommand ExitCommand => field ??= new RelayCommand(() => Application.Current.Exit());

	[field:MaybeNull, AllowNull]
	public static RelayCommand NavigateToSettingsCommand => field ??= new RelayCommand(() => Navigation.ChangePage<SettingsPage>());

	[field:MaybeNull, AllowNull]
	public static RelayCommand NavigateToWrapperCreationPageCommand => field ??= new RelayCommand(() => Navigation.ChangePage<CreateWrapperPage>());

	[field:MaybeNull, AllowNull]
	public static RelayCommand<Wrapper> NavigateToWrapperSettingsCommand => field ??= new RelayCommand<Wrapper>(Navigation.ChangePage<WrapperSettingsPage>);
}