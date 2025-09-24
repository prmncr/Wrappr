using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Utilities;
using Wrappr.Data;
using Wrappr.Model;
using Wrappr.Resources;
using Wrappr.Services;

namespace Wrappr.Components.Pages;

public partial class CreateWrapperPage : INavigable
{
	private readonly ServiceNamePresenterSource _source;

	public CreateWrapperPage()
	{
		InitializeComponent();

		_source = new ServiceNamePresenterSource();
		ServiceNames = new IncrementalLoadingCollection<ServiceNamePresenterSource, ServiceSearchEntry>(_source);

		Loaded += (_, _) =>
		{
			ServiceSearch.Focus(FocusState.Keyboard);
		};
	}

	private IncrementalLoadingCollection<ServiceNamePresenterSource, ServiceSearchEntry> ServiceNames { get; }

	private void ReloadSuggestions(object sender, TextChangedEventArgs e)
	{
		_source.Query = ((TextBox)sender).Text;
		ServiceNames.RefreshAsync();
	}

	private void CreateWrapper(object sender, RoutedEventArgs e)
	{
		if (ServiceList.SelectedItem is not ServiceSearchEntry service)
		{
			Notifications.ShowNearestNotification(
				new Notification(
					Strings.NoServiceSelectedError,
					NotificationSeverity: Notification.Severity.Error
				)
			);
			return;
		}
		var wrapper = new Wrapper(new WrapperConfig(service.ServiceName));
		WrappersStorage.Add(wrapper);
		Navigation.DropCurrentPageAndChange<WrapperSettingsPage>(wrapper);
	}

	private class ServiceNamePresenterSource : IIncrementalSource<ServiceSearchEntry>
	{
		private IEnumerable<ServiceSearchEntry> _source;

		public ServiceNamePresenterSource()
		{
			_source = ReloadSuggestions();
		}

		public string Query
		{
			get;
			set
			{
				field = value;
				_source = ReloadSuggestions();
			}
		} = "";

		public Task<IEnumerable<ServiceSearchEntry>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = new())
		{
			return Task.FromResult(_source.Skip(pageIndex * pageSize).Take(pageSize));
		}

		private List<ServiceSearchEntry> ReloadSuggestions()
		{
			var result = (string.IsNullOrEmpty(Query)
					? Model.Services.GetAll()
					: Model.Services.GetAll().Where(x => x.ServiceName.Contains(Query) || x.DisplayName.Contains(Query)))
				.Select(x => new ServiceSearchEntry { DisplayName = x.DisplayName, ServiceName = x.ServiceName })
				.ToList();

			return result.Count > 0
				? result
				: [new ServiceSearchEntry { IsPlaceholder = true, DisplayName = Strings.NoServiceFoundPlaceholder }];
		}
	}

	private void SelectedServiceChanged(object sender, SelectionChangedEventArgs e)
	{
		CreateButton.IsEnabled = ServiceList.SelectedItem is ServiceSearchEntry;
	}

	public string LocalizedName => Strings.CreateNewWrapperTitle;
}