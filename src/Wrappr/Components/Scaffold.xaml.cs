using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Wrappr.Services;

namespace Wrappr.Components;

[ContentProperty(Name = nameof(Body))]
public partial class Scaffold
{
	public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
		nameof(Title), typeof(string), typeof(Scaffold), new PropertyMetadata(null)
	);

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
		nameof(Actions), typeof(UIElement), typeof(Scaffold), new PropertyMetadata(null)
	);

	public UIElement Actions
	{
		get => (UIElement)GetValue(ActionsProperty);
		set => SetValue(ActionsProperty, value);
	}

	public static readonly DependencyProperty BodyProperty = DependencyProperty.Register(
		nameof(Body), typeof(UIElement), typeof(Scaffold), new PropertyMetadata(null)
	);

	public UIElement Body
	{
		get => (UIElement)GetValue(BodyProperty);
		set => SetValue(BodyProperty, value);
	}

	public Scaffold()
	{
		InitializeComponent();
	}

	private void NavigateToClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
	{
		Navigation.BackTo(args.Index);
	}
}