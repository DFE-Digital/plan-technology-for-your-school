using Dfe.PlanTech.Application.Caching.Interfaces;
using Dfe.PlanTech.Application.Core;
using Dfe.PlanTech.Application.Persistence.Interfaces;
using Dfe.PlanTech.Domain.Caching.Models;
using Dfe.PlanTech.Domain.Questionnaire.Models;

namespace Dfe.PlanTech.Application.Questionnaire.Queries;

public class GetQuestionQuery : ContentRetriever
{
    private readonly IQuestionnaireCacher _cacher;

    public GetQuestionQuery(IQuestionnaireCacher cacher, IContentRepository repository) : base(repository)
    {
        _cacher = cacher;
    }

    public async Task<Question?> GetQuestionById(string id, CancellationToken cancellationToken = default)
    {
        var question = await repository.GetEntityById<Question>(id, 3, cancellationToken);

        return question;
    }

    public Task<Question?> GetQuestionById(string id, string? section, CancellationToken cancellationToken = default)
    {
        if (section != null)
        {
            UpdateSectionTitle(section);
        }

        return GetQuestionById(id, cancellationToken);
    }

    private void UpdateSectionTitle(string section)
    {
        var cached = _cacher.Cached ?? new QuestionnaireCache();
        cached = cached with { CurrentSectionTitle = section };

        _cacher.SaveCache(cached);
    }
}
