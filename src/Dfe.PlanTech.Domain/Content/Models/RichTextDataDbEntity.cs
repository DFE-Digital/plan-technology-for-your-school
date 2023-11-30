using Dfe.PlanTech.Domain.Content.Interfaces;

namespace Dfe.PlanTech.Domain.Content.Models;

/// <summary>
/// Database table for the RichText section
/// </summary>
/// <inheritdoc/>
public class RichTextDataDbEntity : ContentComponentDbEntity, IRichTextData
{
    public string? Uri { get; init; }
}