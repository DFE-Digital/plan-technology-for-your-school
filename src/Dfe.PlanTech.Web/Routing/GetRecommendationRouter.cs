using Dfe.PlanTech.Application.Content.Queries;
using Dfe.PlanTech.Application.Users.Interfaces;
using Dfe.PlanTech.Web.Controllers;
using Dfe.PlanTech.Web.Exceptions;
using Dfe.PlanTech.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PlanTech.Web.Routing;

public class GetRecommendationRouter : IGetRecommendationRouter
{
  private readonly IGetPageQuery _getPageQuery;
  private readonly ILogger<GetRecommendationRouter> _logger;
  private readonly IUser _user;
  private readonly IUserJourneyStatusProcessor _router;

  public GetRecommendationRouter(IGetPageQuery getPageQuery, ILogger<GetRecommendationRouter> logger, IUser user, IUserJourneyStatusProcessor router)
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
      JourneyStatus.Completed => await HandleCompleteStatus(sectionSlug, recommendationSlug, controller, cancellationToken),
      JourneyStatus.CheckAnswers => controller.RedirectToCheckAnswers(sectionSlug),
      JourneyStatus.NotStarted or JourneyStatus.NextQuestion => HandleQuestionStatus(sectionSlug, controller),
      _ => throw new InvalidOperationException($"Invalid journey status - {_router.Status}"),
    };
  }

  /// <summary>
  /// Either render the recommendation page (if correct recommendation for section + maturity),
  /// or redirect user to correct recommendation page
  /// </summary>
  /// <param name="sectionSlug"></param>
  /// <param name="recommendationSlug"></param>
  /// <param name="controller"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="InvalidDataException"></exception>
  /// <exception cref="ContentfulDataUnavailableException"></exception>
  private async Task<IActionResult> HandleCompleteStatus(string sectionSlug,
                                                         string recommendationSlug,
                                                         RecommendationsController controller,
                                                         CancellationToken cancellationToken)
  {
    if (_router.SectionStatus?.Maturity == null) throw new InvalidDataException("Maturity is null, but shouldn't be for a completed section");

    var recommendationForSlug = _router.Section!.Recommendations.FirstOrDefault(recommendation => recommendation.Page.Slug == recommendationSlug) ??
                                  throw new ContentfulDataUnavailableException($"Couldn't find recommendation with slug {recommendationSlug} under '{sectionSlug}'");

    var recommendationForMaturity = _router.Section.GetRecommendationForMaturity(_router.SectionStatus.Maturity) ??
                                    throw new ContentfulDataUnavailableException($"Missing recommendation page for {_router.SectionStatus.Maturity} in section '{sectionSlug}'");

    if (recommendationForMaturity.Sys.Id != recommendationForSlug.Sys.Id)
    {
      return controller.RedirectToAction(RecommendationsController.GetRecommendationAction,
                                         new { sectionSlug, recommendationSlug = recommendationForMaturity.Page.Slug });
    }

    var page = await _getPageQuery.GetPageBySlug(recommendationSlug, cancellationToken);

    var viewModel = new PageViewModel(page, controller, _user, _logger);

    return controller.View("~/Views/Pages/Page.cshtml", viewModel);
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