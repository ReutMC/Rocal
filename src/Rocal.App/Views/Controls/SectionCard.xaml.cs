using System.Windows;
using System.Windows.Controls;

namespace Rocal.App.Views.Controls;

/// <summary>Home-page section: a titled card that shows an empty state until real data exists.</summary>
public partial class SectionCard : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(SectionCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty GlyphProperty =
        DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(SectionCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty EmptyTitleProperty =
        DependencyProperty.Register(nameof(EmptyTitle), typeof(string), typeof(SectionCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty EmptyMessageProperty =
        DependencyProperty.Register(nameof(EmptyMessage), typeof(string), typeof(SectionCard), new PropertyMetadata(string.Empty));

    public SectionCard()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public string EmptyTitle
    {
        get => (string)GetValue(EmptyTitleProperty);
        set => SetValue(EmptyTitleProperty, value);
    }

    public string EmptyMessage
    {
        get => (string)GetValue(EmptyMessageProperty);
        set => SetValue(EmptyMessageProperty, value);
    }
}
