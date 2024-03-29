using Dfe.PlanTech.Domain.Questionnaire.Interfaces;
using Dfe.PlanTech.Domain.Questionnaire.Models;

namespace Dfe.PlanTech.Domain.Responses.Interfaces;

public interface IProcessCheckAnswerDtoCommand
{
    public Task<CheckAnswerDto?> GetCheckAnswerDtoForSection(int establishmentId, ISectionComponent section, CancellationToken cancellationToken = default);
}
