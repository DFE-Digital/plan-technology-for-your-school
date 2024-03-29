using Dfe.PlanTech.Domain.Content.Interfaces;

namespace Dfe.PlanTech.Domain.Content.Models.Buttons;

public class ButtonDbEntity : ContentComponentDbEntity, IButton
{
    public string Value { get; set; } = null!;

    public bool IsStartButton { get; set; }
}
