using Dfe.PlanTech.Application.Persistence.Interfaces;

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
    private void CopySectionsToPage(PageDbEntity page, List<SectionDbEntity> sections)
    {
        var sectionsGroupedByCategory = sections.GroupBy(section => section.CategoryId);

        foreach (var cat in sectionsGroupedByCategory)
        {
            var matching = page.Content.OfType<CategoryDbEntity>()
                                        .FirstOrDefault(category => category != null && category.Id == cat.Key);

            if (matching == null)
            {
                _logger.LogError("Could not find matching category {categoryId} in {pageSlug}", cat.Key, page.Slug);
                continue;
            }
            bool sectionsValid = AllSectionsValid(sections);

            if (!sectionsValid)
            {
                throw new MissingFieldException($"Missing InterstitialPage for sections in {cat.Key}");
            }

            matching.Sections = cat.ToList();
        }
    }

    /// <summary>
    /// Are all sections in this collection valid?
    /// </summary>
    /// <remarks>
    /// Currently just validates that they have an interstitial page, as this will cause errors
    /// on the self-assessment page otherwise currently.
    /// </remarks>
    /// <param name="sections"></param>
    /// <returns>All sections in the enumerable are valid (true) or ANY is invalid (false)</returns>
    private static bool AllSectionsValid(IEnumerable<SectionDbEntity> sections)
        => !sections.Any(cat => cat.InterstitialPage == null);


    /// <summary>
    /// Quer to get <see cref="SectionDbEntity">s for the given page, but with only necessary information we require
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    private IQueryable<SectionDbEntity> SectionsForPageQueryable(PageDbEntity page)
    => _db.Sections.Where(section => section.Category != null && section.Category.ContentPages.Any(categoryPage => categoryPage.Slug == page.Slug))
                .Where(section => section.Order != null)
                .OrderBy(section => section.Order)
                .Select(section => new SectionDbEntity()
                {
                    CategoryId = section.CategoryId,
                    Id = section.Id,
                    Name = section.Name,
                    Questions = section.Questions.Where(question => question.Order != null).OrderBy(question => question.Order).Select(question => new QuestionDbEntity()
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
                        },
                        Id = recommendation.Id
                    }).ToList(),
                    InterstitialPage = new PageDbEntity()
                    {
                        Slug = section.InterstitialPage.Slug,
                        Id = section.InterstitialPage.Id
                    }
                });
}