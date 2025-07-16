using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Wrappr.Services;

namespace Wrappr.Components;

[ContentProperty(Name = nameof(Actions))]
public sealed partial class ActionPanel
{
    public ActionPanel()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
        nameof(Actions), typeof(UIElement), typeof(Scaffold), new PropertyMetadata(null)
    );

    public UIElement Actions
    {
        get => (UIElement)GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    private void NavigateToClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        Navigation.BackTo(args.Index);
    }
}