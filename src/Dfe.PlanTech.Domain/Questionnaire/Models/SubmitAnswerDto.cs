using System.ComponentModel.DataAnnotations;

namespace Dfe.PlanTech.Domain.Questionnaire.Models;

public class SubmitAnswerDto
{
    [Required]
    public string QuestionId { get; init; } = null!;

    [Required]
    public string ChosenAnswerId { get; init; } = null!;

    public string? NextQuestionId { get; init; }
}
