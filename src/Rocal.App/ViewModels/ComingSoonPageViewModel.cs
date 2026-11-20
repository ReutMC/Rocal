using Rocal.App.Services;

namespace Rocal.App.ViewModels;

/// <summary>Honest page for a feature that is not implemented yet. It exposes no actions.</summary>
public sealed class ComingSoonPageViewModel(PageId id, LocalizationService loc, string titleKey, string glyph, int milestone)
    : PageViewModel(id, loc, titleKey, glyph)
{
    public string Description => string.Format(Loc["ComingSoon_Text"], milestone);
}
