using Dfe.PlanTech.Application.Exceptions;
using Dfe.PlanTech.Domain.Content.Queries;
using Dfe.PlanTech.Domain.Submissions.Enums;
using Dfe.PlanTech.Domain.Submissions.Interfaces;
using Dfe.PlanTech.Domain.Users.Interfaces;
using Dfe.PlanTech.Web.Controllers;
using Dfe.PlanTech.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PlanTech.Web.Routing;

public class GetRecommendationRouter : IGetRecommendationRouter
{
    private readonly ISubmissionStatusProcessor _router;
    private readonly IGetAllAnswersForLatestSubmissionQuery _getAllAnswersForLatestSubmissionQuery;
    private readonly IGetSubTopicRecommendationQuery _getSubTopicRecommendationQuery;

    public GetRecommendationRouter(ISubmissionStatusProcessor router, IGetAllAnswersForLatestSubmissionQuery getAllAnswersForLatestSubmissionQuery, IGetSubTopicRecommendationQuery getSubTopicRecommendationQuery)
    {
        _router = router;
        _getAllAnswersForLatestSubmissionQuery = getAllAnswersForLatestSubmissionQuery;
        _getSubTopicRecommendationQuery = getSubTopicRecommendationQuery;
    }

    public async Task<IActionResult> ValidateRoute(string sectionSlug, string recommendationSlug, RecommendationsController controller, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(sectionSlug)) throw new ArgumentNullException(nameof(sectionSlug));
        if (string.IsNullOrEmpty(recommendationSlug)) throw new ArgumentNullException(nameof(recommendationSlug));

        await _router.GetJourneyStatusForSection(sectionSlug, cancellationToken);
        return _router.Status switch
        {
            SubmissionStatus.Completed => await HandleCompleteStatus(controller, cancellationToken),
            SubmissionStatus.CheckAnswers => controller.RedirectToCheckAnswers(sectionSlug),
            SubmissionStatus.NextQuestion => HandleQuestionStatus(sectionSlug, controller),
            SubmissionStatus.NotStarted => PageRedirecter.RedirectToSelfAssessment(controller),
            _ => throw new InvalidOperationException($"Invalid journey status - {_router.Status}"),
        };
    }

    /// <summary>
    /// Render the recommendation page (if correct recommendation for section + maturity),
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="DatabaseException"></exception>
    /// <exception cref="ContentfulDataUnavailableException"></exception>
    private async Task<IActionResult> HandleCompleteStatus(RecommendationsController controller, CancellationToken cancellationToken)
    {
        if (_router.SectionStatus?.Maturity == null) throw new DatabaseException("Maturity is null, but shouldn't be for a completed section");

        if (_router.Section == null) throw new DatabaseException("Section is null, but shouldn't be.");

        var usersAnswers =
            await _getAllAnswersForLatestSubmissionQuery.GetAllAnswersForLatestSubmission(_router.Section.Sys.Id,
                await _router.User.GetEstablishmentId()) ?? throw new DatabaseException($"Could not find users answers for:  {_router.Section.Name}");

        var subTopicRecommendation = await _getSubTopicRecommendationQuery.GetSubTopicRecommendation(_router.Section.Sys.Id, cancellationToken) ?? throw new ContentfulDataUnavailableException($"Could not find subtopic recommendation for:  {_router.Section.Name}");

        var subTopicIntro = subTopicRecommendation.GetRecommendationByMaturity(_router.SectionStatus.Maturity) ?? throw new ContentfulDataUnavailableException($"Could not find recommendation intro for maturity:  {_router.SectionStatus?.Maturity}");

        var subTopicChunks = subTopicRecommendation.Section.GetRecommendationChunksByAnswerIds(usersAnswers.Select(answer => answer.ContentfulRef));

        var viewModel = new RecommendationsViewModel()
        {
            SectionName = subTopicRecommendation.Subtopic.Name,
            Intro = subTopicIntro,
            Chunks = subTopicChunks
        };

        return controller.View("~/Views/Recommendations/Recommendations.cshtml", viewModel);

    }

    /// <summary>
    /// Redirect user to next question in their journey
    /// </summary>
    /// <param name="sectionSlug"></param>
    /// <param name="controller"></param>
    /// <returns></returns>
    private IActionResult HandleQuestionStatus(string sectionSlug, Controller controller)
    => QuestionsController.RedirectToQuestionBySlug(sectionSlug, _router.NextQuestion!.Slug, controller);
}
