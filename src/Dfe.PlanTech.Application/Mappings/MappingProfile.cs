using AutoMapper;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Domain.Content.Models.Buttons;
using Dfe.PlanTech.Domain.Questionnaire.Models;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.PlanTech.Application.Mappings;

[ExcludeFromCodeCoverage]
public class CmsMappingProfile : Profile
{
    public CmsMappingProfile()
    {
        CreateMap<PageDbEntity, Page>();

        CreateMap<ContentComponentDbEntity, ContentComponent>()
        .Include<AnswerDbEntity, Answer>()
        .Include<ButtonDbEntity, Button>()
        .Include<ButtonWithEntryReferenceDbEntity, ButtonWithEntryReference>()
        .Include<ButtonWithLinkDbEntity, ButtonWithLink>()
        .Include<CategoryDbEntity, Category>()
        .Include<HeaderDbEntity, Header>()
        .Include<InsetTextDbEntity, InsetText>()
        .Include<QuestionDbEntity, Question>()
        .Include<RecommendationChunkDbEntity, RecommendationChunk>()
        .Include<RecommendationIntroDbEntity, RecommendationIntro>()
        .Include<RecommendationPageDbEntity, RecommendationPage>()
        .Include<RecommendationSectionDbEntity, RecommendationSection>()
        .Include<SectionDbEntity, Section>()
        .Include<SubTopicRecommendationDbEntity, SubTopicRecommendation>()
        .Include<TextBodyDbEntity, TextBody>()
        .Include<TitleDbEntity, Title>()
        .Include<WarningComponentDbEntity, WarningComponent>()
        .ForMember(dest => dest.Sys, opt => opt.MapFrom(src => src));

        CreateMap<ContentComponentDbEntity, SystemDetails>();

        CreateMap<AnswerDbEntity, Answer>();
        CreateMap<ButtonDbEntity, Button>();
        CreateMap<ButtonWithEntryReferenceDbEntity, ButtonWithEntryReference>();
        CreateMap<ButtonWithLinkDbEntity, ButtonWithLink>();
        CreateMap<CategoryDbEntity, Category>();
        CreateMap<HeaderDbEntity, Header>();
        CreateMap<InsetTextDbEntity, InsetText>();
        CreateMap<QuestionDbEntity, Question>();
        CreateMap<RecommendationChunkDbEntity, RecommendationChunk>();
        CreateMap<RecommendationIntroDbEntity, RecommendationIntro>();
        CreateMap<RecommendationPageDbEntity, RecommendationPage>();
        CreateMap<RecommendationSectionDbEntity, RecommendationSection>();
        CreateMap<SectionDbEntity, Section>();
        CreateMap<SubTopicRecommendationDbEntity, SubTopicRecommendation>();
        CreateMap<TextBodyDbEntity, TextBody>();
        CreateMap<TitleDbEntity, Title>();
        CreateMap<WarningComponentDbEntity, WarningComponent>();

        CreateMap<RichTextContentDbEntity, RichTextContent>();
        CreateMap<RichTextDataDbEntity, RichTextData>();
        CreateMap<RichTextMarkDbEntity, RichTextMark>();
    }
}
