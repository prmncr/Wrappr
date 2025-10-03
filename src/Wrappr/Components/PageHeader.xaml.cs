using System.Windows.Input;
using Microsoft.UI.Xaml;
using Wrappr.Services;

namespace Wrappr.Components;

public partial class PageHeader
{
	public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
		nameof(Title), typeof(string), typeof(PageHeader), new PropertyMetadata(null)
	);

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public static readonly DependencyProperty OnBackButtonClickProperty = DependencyProperty.Register(
		nameof(OnBackButtonClick), typeof(ICommand), typeof(PageHeader), new PropertyMetadata(null)
	);

	public ICommand OnBackButtonClick
	{
		get => (ICommand)GetValue(OnBackButtonClickProperty);
		set => SetValue(OnBackButtonClickProperty, value);
	}

	public PageHeader()
	{
		InitializeComponent();
	}

	private void BackButtonClick(object sender, RoutedEventArgs e)
	{
		Navigation.Back();
	}
}