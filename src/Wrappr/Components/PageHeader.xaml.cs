using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Services;

namespace Wrappr.Components;

public sealed partial class PageHeader
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(PageHeader), new PropertyMetadata(null)
    );

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
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