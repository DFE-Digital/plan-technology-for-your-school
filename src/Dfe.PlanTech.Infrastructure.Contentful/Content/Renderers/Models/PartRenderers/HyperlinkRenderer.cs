using System.Text;
using Dfe.PlanTech.Domain.Content.Interfaces;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Infrastructure.Contentful.Content.Renderers.Interfaces;
using Dfe.PlanTech.Infrastructure.Contentful.Content.Renderers.Options;

namespace Dfe.PlanTech.Infrastructure.Contentful.Content.Renderers.Models.PartRenderers;

public class HyperlinkRenderer : BaseRichTextContentPartRender
{
    private readonly HyperlinkRendererOptions _options;

    public HyperlinkRenderer(HyperlinkRendererOptions options) : base(RichTextNodeType.Hyperlink)
    {
        _options = options;
    }

    public override StringBuilder AddHtml(IRichTextContent content, IRichTextContentPartRendererCollection renderers, StringBuilder stringBuilder)
    {
        if (string.IsNullOrEmpty(content.Data?.Uri))
        {
            //TODO: Log missing data;
            return stringBuilder;
        }

        stringBuilder.Append("<a href=\"");
        stringBuilder.Append(content.Data.Uri);

        if (_options.Classes != null)
        {
            _options.AddClasses(stringBuilder);
        }

        stringBuilder.Append("\">");

        RenderChildren(content, renderers, stringBuilder);

        stringBuilder.Append(content.Value);

        stringBuilder.Append("</a>");
        return stringBuilder;
    }
}