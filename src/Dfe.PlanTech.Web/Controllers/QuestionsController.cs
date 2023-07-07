using Dfe.PlanTech.Application.Caching.Interfaces;
using Dfe.PlanTech.Application.Questionnaire.Queries;
using Dfe.PlanTech.Application.Submission.Interfaces;
using Dfe.PlanTech.Domain.Answers.Models;
using Dfe.PlanTech.Domain.Questions.Models;
using Dfe.PlanTech.Domain.Questionnaire.Models;
using Dfe.PlanTech.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PlanTech.Web.Controllers;

[Authorize]
[Route("/question")]
public class QuestionsController : BaseController<QuestionsController>
{
    public QuestionsController(ILogger<QuestionsController> logger, IUrlHistory history) : base(logger, history) { }

    private async Task<Domain.Questionnaire.Models.Question> _GetQuestion(string id, string? section, [FromServices] GetQuestionQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
        return await query.GetQuestionById(id, section, cancellationToken) ?? throw new KeyNotFoundException($"Could not find question with id {id}");
    }

    [HttpGet("{id?}")]
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="section">Name of current section (if starting new)</param>
    /// <param name="query"></param>
    /// <exception cref="ArgumentNullException">Throws exception when Id is null or empty</exception>
    /// <returns></returns>
    public async Task<IActionResult> GetQuestionById(string id, string? section, [FromServices] GetQuestionQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
        string parameters = string.Empty;

        if (TempData.ContainsKey("param"))
            parameters = TempData["Param"].ToString();

        var question = await _GetQuestion(id, section, query, cancellationToken);

        var viewModel = new QuestionViewModel()
        {
            Question = question,
            BackUrl = history.LastVisitedUrl?.ToString() ?? "self-assessment"
        };

        return View("Question", viewModel);
    }

    private async Task<String?> _GetQuestionTextById(String questionId)
    {
        var question = await _GetQuestion(questionId, null, HttpContext.RequestServices.GetRequiredService<GetQuestionQuery>(), CancellationToken.None);
        return question.Text;
    }

    private async Task<String?> _GetAnswerTextById(String questionId, String chosenAnswerId)
    {
        var question = await _GetQuestion(questionId, null, HttpContext.RequestServices.GetRequiredService<GetQuestionQuery>(), CancellationToken.None);
        foreach (var answer in question.Answers)
        {
            if (answer.Sys?.Id == chosenAnswerId) return answer.Text;
        }
        return null;
    }

    private async Task _RecordQuestion(RecordQuestionDto recordQuestionDto)
    {
        if (recordQuestionDto.QuestionText == null) throw new ArgumentNullException(nameof(recordQuestionDto.QuestionText));
        IRecordQuestionCommand recordQuestionCommand = HttpContext.RequestServices.GetRequiredService<IRecordQuestionCommand>();
        await recordQuestionCommand.RecordQuestion(recordQuestionDto);
    }

    private async Task _RecordAnswer(RecordAnswerDto recordAnswerDto)
    {
        if (recordAnswerDto.AnswerText == null) throw new ArgumentNullException(nameof(recordAnswerDto.AnswerText));
        IRecordAnswerCommand recordAnswerCommand = HttpContext.RequestServices.GetRequiredService<IRecordAnswerCommand>();
        await recordAnswerCommand.RecordAnswer(recordAnswerDto);
    }

    [HttpPost("SubmitAnswer")]
    public async Task<IActionResult> SubmitAnswer(SubmitAnswerDto submitAnswerDto)
    {
        if (submitAnswerDto == null) throw new ArgumentNullException(nameof(submitAnswerDto));

        if (!ModelState.IsValid) return RedirectToAction("GetQuestionById", new { id = submitAnswerDto.QuestionId });

        await _RecordQuestion(new RecordQuestionDto() { QuestionText = await _GetQuestionTextById(submitAnswerDto.QuestionId), ContentfulRef = submitAnswerDto.QuestionId });
        await _RecordAnswer(new RecordAnswerDto() { AnswerText = await _GetAnswerTextById(submitAnswerDto.QuestionId, submitAnswerDto.ChosenAnswerId), ContentfulRef = submitAnswerDto.ChosenAnswerId });

        if (string.IsNullOrEmpty(submitAnswerDto.NextQuestionId)) return RedirectToAction("GetByRoute", "Pages", new { route = "check-answers" });
        else return RedirectToAction("GetQuestionById", new { id = submitAnswerDto.NextQuestionId });
    }
}
