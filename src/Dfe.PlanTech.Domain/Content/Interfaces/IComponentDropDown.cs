namespace Dfe.PlanTech.Domain.Content.Interfaces;

/// <summary>
/// Model for DropDown type.
/// </summary>
public interface IComponentDropDown
{
    /// <summary>
    /// The title to display.
    /// </summary>
    public string Title { get; }
}

public interface IComponentDropDown<TContent>
where TContent : IRichTextContent
{

    /// <summary>
    /// The Content to display.
    /// </summary>
    public TContent Content { get; }
}