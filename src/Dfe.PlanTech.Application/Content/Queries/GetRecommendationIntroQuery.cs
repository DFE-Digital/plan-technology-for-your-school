using Dfe.PlanTech.Domain.Content.Queries;
using Dfe.PlanTech.Domain.Questionnaire.Enums;
using Dfe.PlanTech.Domain.Questionnaire.Models;

namespace Dfe.PlanTech.Application.Content.Queries;

public class GetRecommendationIntroQuery(IGetSubTopicRecommendation getSubTopicRecommendationFromContentfulQuery) : IGetRecommendationIntro
{
    private readonly IGetSubTopicRecommendation _getSubTopicRecommendationFromContentfulQuery = getSubTopicRecommendationFromContentfulQuery;

    public async Task<RecommendationIntro> GetRecommendationIntroForSubtopic(Section subTopic, Maturity maturity, CancellationToken cancellationToken = default)
        => (await _getSubTopicRecommendationFromContentfulQuery.GetSubTopicRecommendation(subTopic, cancellationToken)).Intros.Where(intro => intro.Maturity.Equals(maturity.ToString())).First();
}