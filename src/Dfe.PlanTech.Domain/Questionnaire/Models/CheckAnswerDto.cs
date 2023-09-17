using System.ComponentModel.DataAnnotations;

namespace Dfe.PlanTech.Domain.Questionnaire.Models;

public class CheckAnswerDto
{
    [Required]
    public List<QuestionWithAnswer> Responses { get; init; } = new List<QuestionWithAnswer>();

    public int SubmissionId { get; init; }
}