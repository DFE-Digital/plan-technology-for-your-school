using Dfe.PlanTech.Application.Persistence.Interfaces;
using Dfe.PlanTech.Domain.Content.Interfaces;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Domain.Questionnaire.Models;
using Microsoft.Extensions.Logging;

namespace Dfe.PlanTech.Application.Content.Queries;

public class GetCategorySectionsQuery : IGetPageChildrenQuery
{
    private readonly ICmsDbContext _db;
    private readonly ILogger<GetCategorySectionsQuery> _logger;

    public GetCategorySectionsQuery(ICmsDbContext db, ILogger<GetCategorySectionsQuery> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// If there are any "Category" components in the Page.Content, then load the required Section information for each one.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task TryLoadChildren(PageDbEntity page, CancellationToken cancellationToken)
    {
        try
        {
            var pageHasCategories = page.Content.Exists(content => content is CategoryDbEntity);

            if (!pageHasCategories) return;

            var sections = await _db.ToListAsync(SectionsForPageQueryable(page), cancellationToken);

            CopySectionsToPage(page, sections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching categories for {page}", page.Id);
            throw;
        }
    }

    /// <summary>
    /// Copies the retrieved Sections from the database over the corresponding Category's section
    /// </summary>
    /// <param name="page">Page that contains categories</param>
    /// <param name="sections">"Complete" section from database</param>
    private static void CopySectionsToPage(PageDbEntity page, List<SectionDbEntity> sections)
    {
        var sectionsGroupedByCategory = sections.GroupBy(section => section.CategoryId);

        foreach (var cat in sectionsGroupedByCategory)
        {
            var matching = page.Content.OfType<CategoryDbEntity>()
                                        .FirstOrDefault(category => category != null && category.Id == cat.Key);

            if (matching == null)
            {
                continue;
            }

            matching.Sections = cat.ToList();
        }
    }


    /// <summary>
    /// Quer to get <see cref="SectionDbEntity">s for the given page, but with only necessary information we require
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    private IQueryable<SectionDbEntity> SectionsForPageQueryable(PageDbEntity page)
    => _db.Sections.Where(section => section.Category != null && section.Category.ContentPages.Any(categoryPage => categoryPage.Slug == page.Slug))
                .Select(section => new SectionDbEntity()
                {
                    CategoryId = section.CategoryId,
                    Id = section.Id,
                    Name = section.Name,
                    Questions = section.Questions.Select(question => new QuestionDbEntity()
                    {
                        Slug = question.Slug,
                        Id = question.Id,
                    }).ToList(),
                    Recommendations = section.Recommendations.Select(recommendation => new RecommendationPageDbEntity()
                    {
                        DisplayName = recommendation.DisplayName,
                        Maturity = recommendation.Maturity,
                        Page = new PageDbEntity()
                        {
                            Slug = recommendation.Page.Slug
                        }
                    }).ToList(),
                    InterstitialPage = section.InterstitialPage == null ? null : new PageDbEntity()
                    {
                        Slug = section.InterstitialPage.Slug,
                        Id = section.InterstitialPage.Id
                    }
                });
}