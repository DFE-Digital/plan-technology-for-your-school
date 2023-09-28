using Dfe.PlanTech.Application.Content.Queries;
using Dfe.PlanTech.Application.Users.Interfaces;
using Dfe.PlanTech.Web.Controllers;
using Dfe.PlanTech.Web.Exceptions;
using Dfe.PlanTech.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PlanTech.Web.Routing;

public class GetRecommendationValidator
{
  private readonly IGetPageQuery _getPageQuery;
  private readonly ILogger<GetRecommendationValidator> _logger;
  private readonly IUser _user;
  private readonly UserJourneyRouter _router;

  public GetRecommendationValidator(IGetPageQuery getPageQuery, ILogger<GetRecommendationValidator> logger, IUser user, UserJourneyRouter router)
  {
    _getPageQuery = getPageQuery;
    _logger = logger;
    _user = user;
    _router = router;
  }

  public async Task<IActionResult> ValidateRoute(string sectionSlug,
                                                 string recommendationSlug,
                                                 RecommendationsController controller,
                                                 CancellationToken cancellationToken)
  {
    if (string.IsNullOrEmpty(sectionSlug)) throw new ArgumentException($"'{nameof(sectionSlug)}' cannot be null or empty.");
    if (string.IsNullOrEmpty(recommendationSlug)) throw new ArgumentException($"'{nameof(recommendationSlug)}' cannot be null or empty.");

    await _router.GetJourneyStatusForSection(sectionSlug, cancellationToken);
    return _router.Status switch
    {
      JourneyStatus.Completed => await HandleCompleteStatus(sectionSlug, recommendationSlug, _router, controller, cancellationToken),
      JourneyStatus.CheckAnswers => controller.RedirectToCheckAnswers(sectionSlug),
      JourneyStatus.NotStarted or JourneyStatus.NextQuestion => HandleQuestionStatus(sectionSlug, _router, controller),
      _ => throw new InvalidOperationException($"Invalid journey status - {_router.Status}"),
    };
  }

  private async Task<IActionResult> HandleCompleteStatus(string sectionSlug, string recommendationSlug, UserJourneyRouter router, RecommendationsController controller, CancellationToken cancellationToken)
  {
    if (router.SectionStatus?.Maturity == null) throw new InvalidDataException("Maturity is null - shouldn't be");

    var recommendationForSlug = router.Section!.Recommendations.FirstOrDefault(recommendation => recommendation.Page.Slug == recommendationSlug) ??
                                  throw new ContentfulDataUnavailableException($"Couldn't find recommendation with slug {recommendationSlug} under {sectionSlug}");

    var recommendationForMaturity = router.Section.GetRecommendationForMaturity(router.SectionStatus.Maturity) ??
                                    throw new ContentfulDataUnavailableException("Missing recommendation page");

    if (recommendationForMaturity.Sys.Id != recommendationForSlug.Sys.Id)
    {
      return controller.RedirectToAction(RecommendationsController.GetRecommendationAction,
                                         new { sectionSlug, recommendationSlug = recommendationForMaturity.Page.Slug });
    }

    var page = await _getPageQuery.GetPageBySlug(recommendationSlug, cancellationToken);

    var viewModel = new PageViewModel(page, controller, _user, _logger);

    return controller.View("~/Views/Pages/Page.cshtml", viewModel);
  }

  private static IActionResult HandleQuestionStatus(string sectionSlug, UserJourneyRouter router, Controller controller)
  => QuestionsController.RedirectToQuestionBySlug(sectionSlug, router.NextQuestion!.Slug, controller);
}