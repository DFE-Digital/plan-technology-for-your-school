using Dfe.PlanTech.Domain.Content.Interfaces;

namespace Dfe.PlanTech.Domain.Content.Models.Buttons;

/// <summary>
/// A button that links to a different entry
/// </summary>
public class ButtonWithEntryReferenceDbEntity : ContentComponentDbEntity, IButtonWithEntryReference<ButtonDbEntity, ContentComponentDbEntity>
{
    public ButtonDbEntity Button { get; set; } = null!;

    public string ButtonId { get; set; } = null!;

    public ContentComponentDbEntity LinkToEntry { get; set; } = null!;

    public string LinkToEntryId { get; set; } = null!;
}

