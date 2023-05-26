using System.Text;
using Dfe.PlanTech.Domain.Content.Interfaces;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Infrastructure.Contentful.Content.Renderers.Options;

namespace Dfe.PlanTech.Infrastructure.Contentful.Content;

public class ParagraphRenderer : RichTextContentRender
{
    private readonly ParagraphRendererOptions _options;
    public ParagraphRenderer(ParagraphRendererOptions options) : base(RichTextNodeType.Paragraph)
    {
        _options = options;
    }

    public override StringBuilder AddHtml(IRichTextContent content, IRichTextContentPartRendererCollection renderers, StringBuilder stringBuilder)
    {
        if (_options.Classes == null)
        {
            stringBuilder.Append("<p>");
        }
        else
        {
            stringBuilder.Append("<p");
            _options.AddClasses(stringBuilder);
            stringBuilder.Append("\">");
        }

        RenderChildren(content, renderers, stringBuilder);

        stringBuilder.Append("</p>");
        return stringBuilder;
    }
}