using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Data;
using Wrappr.Model;
using Wrappr.Resources;
using Wrappr.Services;

namespace Wrappr.Components.Pages;

public sealed partial class CreateWrapperPage {
	private readonly ServiceNamePresenterSource _source;

	public CreateWrapperPage() {
		InitializeComponent();

		_source = new ServiceNamePresenterSource();
		ServiceNames = new IncrementalLoadingCollection<ServiceNamePresenterSource, ServiceNamePresenter>(_source);

		Loaded += (_, _) => {
			ServiceSearch.Focus(FocusState.Keyboard);
		};
	}

	private IncrementalLoadingCollection<ServiceNamePresenterSource, ServiceNamePresenter> ServiceNames { get; }

	private void ReloadSuggestions(object sender, TextChangedEventArgs e) {
		_source.Query = ((TextBox)sender).Text;
		ServiceNames.RefreshAsync();
	}

	private void CreateWrapper(object sender, RoutedEventArgs e) {
		if (ServiceList.SelectedItem is not ServiceNamePresenter service) {
			throw new Exception();
		}
		var wrapper = new Wrapper(new WrapperConfig(service.ServiceName));
		Wrappers.Instance.Storage.Add(wrapper);
		Navigation.ChangePageAndRemovePrevious<WrapperSettingsPage>();
		Navigation.CurrentPage!.DataContext = wrapper;
	}

	private class ServiceNamePresenterSource : IIncrementalSource<ServiceNamePresenter> {
		private IEnumerable<ServiceNamePresenter> _source;

		public ServiceNamePresenterSource() {
			_source = ReloadSuggestions();
		}

		public string Query
		{
			get;
			set {
				field = value;
				_source = ReloadSuggestions();
			}
		} = "";

		public Task<IEnumerable<ServiceNamePresenter>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = new()) {
			return Task.FromResult(_source.Skip(pageIndex * pageSize).Take(pageSize));
		}

		private List<ServiceNamePresenter> ReloadSuggestions() {
			var result = (string.IsNullOrEmpty(Query)
					? Model.Services.GetAll()
					: Model.Services.GetAll().Where(x => x.ServiceName.Contains(Query) || x.DisplayName.Contains(Query)))
				.Select(x => new ServiceNamePresenter { DisplayName = x.DisplayName, ServiceName = x.ServiceName })
				.ToList();

			return result.Count > 0
				? result
				: [new ServiceNamePresenter { IsPlaceholder = true, DisplayName = Strings.NoServiceFoundPlaceholder }];
		}
	}
}