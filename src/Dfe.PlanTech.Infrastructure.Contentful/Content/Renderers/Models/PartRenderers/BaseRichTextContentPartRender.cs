using Dfe.PlanTech.Domain.Content.Enums;
using Dfe.PlanTech.Domain.Content.Interfaces;
using Dfe.PlanTech.Domain.Content.Models;
using System.Text;

namespace Dfe.PlanTech.Infrastructure.Contentful.Content.Renderers.Models.PartRenderers;

public abstract class BaseRichTextContentPartRender : IRichTextContentPartRenderer
{
    private readonly RichTextNodeType _nodeType;

    protected BaseRichTextContentPartRender(RichTextNodeType nodeType)
    {
        _nodeType = nodeType;
    }

    public bool Accepts(RichTextContent content) => content.MappedNodeType == _nodeType;

    public abstract StringBuilder AddHtml(RichTextContent content, IRichTextContentPartRendererCollection rendererCollection, StringBuilder stringBuilder);
}
