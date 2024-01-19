using AutoMapper;
using Dfe.PlanTech.Application.Content.Queries;
using Dfe.PlanTech.Application.Persistence.Interfaces;
using Dfe.PlanTech.Domain.Content.Interfaces;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Domain.Content.Models.Buttons;
using Dfe.PlanTech.Domain.Questionnaire.Enums;
using Dfe.PlanTech.Domain.Questionnaire.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Dfe.PlanTech.Application.UnitTests.Content.Queries;

public class GetPageFromDbQueryTests
{
    private const string TEST_PAGE_SLUG = "test-page-slug";
    private const string SECTION_SLUG = "SectionSlugTest";
    private const string SECTION_TITLE = "SectionTitleTest";
    private const string LANDING_PAGE_SLUG = "LandingPage";
    private const string BUTTON_REF_SLUG = "ButtonReferences";
    private const string CATEGORY_ID = "category-one";
    private readonly ICmsDbContext _cmsDbSubstitute = Substitute.For<ICmsDbContext>();
    private readonly ILogger<GetPageFromDbQuery> _logger = Substitute.For<ILogger<GetPageFromDbQuery>>();
    private readonly IMapper _mapperSubstitute = Substitute.For<IMapper>();
    private readonly GetPageFromDbQuery _getPageFromDbQuery;

    private readonly List<Page> _pages = new() {
        new Page(){
            Slug = "Index"
        },
        new Page(){
            Slug = LANDING_PAGE_SLUG,
        },
        new Page(){
            Slug = "AuditStart"
        },
        new Page(){
            Slug = SECTION_SLUG,
            DisplayTopicTitle = true,
            DisplayHomeButton= false,
            DisplayBackButton = false,
        },
        new Page(){
            Slug = TEST_PAGE_SLUG,
            Sys = new() {
                Id = "test-page-id"
            }
        },
    };

    private readonly List<PageDbEntity> _pagesFromDb = new() {
        new PageDbEntity(){
            Slug = "Index",
            Content = new(){
                new HeaderDbEntity()
            }
        },
        new PageDbEntity(){
            Slug = TEST_PAGE_SLUG,
            Id = "test-page-id"
        },
        new PageDbEntity(){
            Slug = LANDING_PAGE_SLUG,
            Content = new(){
                new HeaderDbEntity(),
            }
        },
        new PageDbEntity(){
            Slug = "AuditStart",
        },
        new PageDbEntity()
    {
        Slug = SECTION_SLUG,
            DisplayTopicTitle = true,
            DisplayHomeButton = false,
            DisplayBackButton = false,
        },
        _pageWithButton,
        _pageWithCategories,
        _pageWithRichTextContent,
    };

    private readonly static PageDbEntity _pageWithCategories = new()
    {
        Slug = "categories_page",
        Id = CATEGORY_ID,
        Content = new()
        {
        }
    };

    private readonly static CategoryDbEntity _category = new()
    {
        Id = CATEGORY_ID,
        ContentPages = new()
        {

        },
        Sections = new()
        {

        }
    };

    private readonly static List<SectionDbEntity> _sections = new(){
                new SectionDbEntity(){
                    Name = "sSection one",
                    CategoryId = CATEGORY_ID,
                    Order = 0,
                    Recommendations = new(){
                        new RecommendationPageDbEntity(){
                            DisplayName = "Recommendation one",
                            Maturity = Maturity.High,
                            Page = new(){
                                Slug = "recommendation-high"
                            }
                        },
                        new RecommendationPageDbEntity(){
                            DisplayName = "Recommendation two",
                            Maturity = Maturity.Medium,
                            Page = new(){
                                Slug = "recommendation-medium"
                            }
                        }
                    },
                    Questions = new(){
                        new QuestionDbEntity(){
                            Order = 0,
                            Slug = "question-one-slug",
                        },
                        new QuestionDbEntity(){
                            Order = 1,
                            Slug = "question-two-slug"
                        }
                    }
                },
            };

    private readonly static PageDbEntity _pageWithButton = new()
    {
        Slug = BUTTON_REF_SLUG,
        Content = new(){
                new ButtonWithEntryReferenceDbEntity(){
                    Button = new ButtonDbEntity(){
                        Value = "Button value",
                        IsStartButton = true
                    }
                }
            }
    };

    private static readonly PageDbEntity _pageWithRichTextContent = new()
    {
        Slug = "rich_text_content",
        Content = new()
        {
            new TextBodyDbEntity(){
                RichTextId = 1
            },
            new TextBodyDbEntity(){
                RichTextId = 2
            },
        }
    };

    private readonly IGetPageChildrenQuery _getPageChildrenQuery = Substitute.For<IGetPageChildrenQuery>();

    public GetPageFromDbQueryTests()
    {
        _cmsDbSubstitute.ToListAsync(Arg.Any<IQueryable<SectionDbEntity>>())
                        .Returns(callinfo =>
                        {
                            var queryable = callinfo.ArgAt<IQueryable<SectionDbEntity>>(0);

                            return queryable.ToList();
                        });

        _mapperSubstitute.Map<PageDbEntity, Page>(Arg.Any<PageDbEntity>())
                        .Returns(callinfo =>
                        {
                            var page = callinfo.ArgAt<PageDbEntity>(0);

                            return new Page()
                            {
                                Slug = page.Slug,
                                DisplayBackButton = page.DisplayBackButton,
                                DisplayHomeButton = page.DisplayBackButton,
                                DisplayOrganisationName = page.DisplayOrganisationName,
                                DisplayTopicTitle = page.DisplayTopicTitle,
                                Content = page.Content.Select(content =>
                                {
                                    if (content is CategoryDbEntity category)
                                    {
                                        return new Category()
                                        {
                                            Sys = new()
                                            {
                                                Id = category.Id,
                                            },
                                            Sections = category.Sections.Select(section => new Section()
                                            {
                                                Sys = new()
                                                {
                                                    Id = section.Id
                                                },
                                                Recommendations = section.Recommendations.Select(recommendation => new RecommendationPage()
                                                {
                                                    Sys = new()
                                                    {
                                                        Id = recommendation.Id
                                                    }
                                                }).ToList(),
                                                Questions = section.Questions.Select(question => new Question()
                                                {
                                                    Sys = new()
                                                    {
                                                        Id = question.Id
                                                    }
                                                }).ToList(),
                                            }).ToList()
                                        } as ContentComponent;
                                    }

                                    return new TextBody();
                                }).ToList()
                            };
                        });

        foreach (var section in _sections)
        {
            section.Category = _category;
            _category.Sections.Add(new()
            {
                Id = section.Id,
                Category = _category,
                CategoryId = _category.Id
            });
        }

        _category.ContentPages = new(){
            _pageWithCategories
        };

        _pageWithCategories.Content.Add(_category);

        _cmsDbSubstitute.Sections.Returns(_sections.AsQueryable());

        _getPageChildrenQuery.TryLoadChildren(Arg.Any<PageDbEntity>(), Arg.Any<CancellationToken>())
                            .Returns(Task.CompletedTask);

        _getPageFromDbQuery = new GetPageFromDbQuery(_cmsDbSubstitute, _logger, _mapperSubstitute, new[] { _getPageChildrenQuery });
    }

    [Fact]
    public async Task Should_Retrieve_Page_By_Slug_From_Db()
    {
        _cmsDbSubstitute.Pages.Returns(_pagesFromDb.AsQueryable());
        _cmsDbSubstitute.GetPageBySlug(Arg.Any<string>(), Arg.Any<CancellationToken>())
                        .Returns(callinfo =>
                        {
                            var slug = callinfo.ArgAt<string>(0);

                            return _cmsDbSubstitute.Pages.FirstOrDefault(page => string.Equals(page.Slug, slug));
                        });

        var result = await _getPageFromDbQuery.GetPageBySlug(LANDING_PAGE_SLUG);

        Assert.NotNull(result);
        Assert.Equal(LANDING_PAGE_SLUG, result.Slug);

        await _cmsDbSubstitute.ReceivedWithAnyArgs(1).GetPageBySlug(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _cmsDbSubstitute.ReceivedWithAnyArgs(0).ToListAsync(Arg.Any<IQueryable<SectionDbEntity>>(), Arg.Any<CancellationToken>());
        await _cmsDbSubstitute.ReceivedWithAnyArgs(0).ToListAsync(Arg.Any<IQueryable<ButtonWithEntryReferenceDbEntity>>(), Arg.Any<CancellationToken>());
        _cmsDbSubstitute.ReceivedWithAnyArgs(0).RichTextContentsByPageSlug(Arg.Any<string>());
        await _getPageChildrenQuery.ReceivedWithAnyArgs(1).TryLoadChildren(Arg.Any<PageDbEntity>(), Arg.Any<CancellationToken>());
    }
}