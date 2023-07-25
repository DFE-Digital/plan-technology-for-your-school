﻿using Dfe.PlanTech.Application.Caching.Interfaces;
using Dfe.PlanTech.Application.Core;
using Dfe.PlanTech.Application.Persistence.Interfaces;
using Dfe.PlanTech.Application.Persistence.Models;
using Dfe.PlanTech.Domain.Questionnaire.Models;
using Microsoft.AspNetCore.Http.Extensions;

namespace Dfe.PlanTech.Application.Recommendation.Queries
{
    public class GetRecommendationQuery : ContentRetriever
    {
        public GetRecommendationQuery(IContentRepository repository) : base(repository) { }

        public async Task<IEnumerable<RecommendationPage>> GetRecommendations()
        {
            var options = new GetEntitiesOptions();
            return await repository.GetEntities<RecommendationPage>("recommendationPage", options);
        }
    }
}
