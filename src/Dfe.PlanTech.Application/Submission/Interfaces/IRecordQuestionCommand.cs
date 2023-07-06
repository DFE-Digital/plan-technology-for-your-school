using Dfe.PlanTech.Domain.Questions.Models;

namespace Dfe.PlanTech.Application.Submission.Interfaces;

public interface IRecordQuestionCommand
{
    Task RecordQuestion(RecordQuestionDto recordQuestionDto);
}
