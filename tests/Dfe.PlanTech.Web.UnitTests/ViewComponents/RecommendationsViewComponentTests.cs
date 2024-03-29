using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Domain.Interfaces;
using Dfe.PlanTech.Domain.Questionnaire.Enums;
using Dfe.PlanTech.Domain.Questionnaire.Models;
using Dfe.PlanTech.Web.Models;
using Dfe.PlanTech.Web.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Dfe.PlanTech.Web.UnitTests.ViewComponents
{
    public class RecommendationsViewComponentTests
    {
        private readonly RecommendationsViewComponent _recommendationsComponent;
        private readonly Category _category;
        private readonly Category _categoryTwo;
        private readonly IGetSubmissionStatusesQuery _getSubmissionStatusesQuery;
        private readonly ILogger<RecommendationsViewComponent> _loggerCategory;

        public RecommendationsViewComponentTests()
        {
            _getSubmissionStatusesQuery = Substitute.For<IGetSubmissionStatusesQuery>();
            _loggerCategory = Substitute.For<ILogger<RecommendationsViewComponent>>();

            _recommendationsComponent = new RecommendationsViewComponent(_loggerCategory, _getSubmissionStatusesQuery);

            _category = new Category()
            {
                Completed = 1,
                Sections =
                [
                    new()
                    {
                        Sys = new SystemDetails() { Id = "Section1" },
                        Name = "Test Section 1",
                        Recommendations = [
                            new RecommendationPage()
                            {
                                InternalName = "High-Maturity-Recommendation-Page-InternalName",
                                DisplayName = "High-Maturity-Recommendation-Page-DisplayName",
                                Maturity = Maturity.High,
                                Page = new Page() { Slug = "High-Maturity-Recommendation-Page-Slug" }
                            }
                        ],
                        InterstitialPage = new Page
                        {
                            Slug = "test-slug"
                        }
                    }
                ]
            };

            _categoryTwo = new Category()
            {
                Completed = 1,
                Sections = [
                    new()
                    {
                        Sys = new SystemDetails() { Id = "Section1" },
                        Name = "Test Section 1",
                        Recommendations = [
                            new()
                            {
                                InternalName = "High-Maturity-Recommendation-Page-InternalName-Two",
                                DisplayName = "High-Maturity-Recommendation-Page-DisplayName-Twp",
                                Maturity = Maturity.High,
                                Page = new Page() { Slug = "High-Maturity-Recommendation-Page-Slug-Two" }
                            }
                        ],
                        InterstitialPage = new Page
                        {
                            Slug = "test-slug"
                        }
                    }
                ]
            };
        }

        [Fact]
        public async Task Returns_RecommendationInfo_If_It_Exists_ForMaturity()
        {
            _category.SectionStatuses.Add(new Domain.Submissions.Models.SectionStatusDto()
            {
                SectionId = "Section1",
                Completed = 1,
                Maturity = "High"
            });

            Category[] categories = new Category[] { _category };

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns(_category.SectionStatuses.ToList());

            var result = await _recommendationsComponent.InvokeAsync(categories) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as IEnumerable<RecommendationsViewComponentViewModel>;
            Assert.NotNull(unboxed);

            unboxed = unboxed.ToList();
            Assert.NotEmpty(unboxed);

            Assert.Equal(_category.Sections[0].Recommendations[0].Page.Slug, unboxed.First().RecommendationSlug);
            Assert.Equal(_category.Sections[0].Recommendations[0].DisplayName, unboxed.First().RecommendationDisplayName);
            Assert.Null(unboxed.First().NoRecommendationFoundErrorMessage);
        }

        [Fact]
        public async Task Returns_RecommendationInfo_For_Multiple_Categories_If_It_Exists_ForMaturity()
        {
            _category.SectionStatuses.Add(new Domain.Submissions.Models.SectionStatusDto()
            {
                SectionId = "Section1",
                Completed = 1,
                Maturity = "High"
            });
            _categoryTwo.SectionStatuses.Add(new Domain.Submissions.Models.SectionStatusDto()
            {
                SectionId = "Section1",
                Completed = 1,
                Maturity = "High"
            });

            Category[] categories = [_category, _categoryTwo];

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns([.. _category.SectionStatuses]);
            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_categoryTwo.Sections).Returns([.. _categoryTwo.SectionStatuses]);

            var result = await _recommendationsComponent.InvokeAsync(categories) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as IEnumerable<RecommendationsViewComponentViewModel>;
            Assert.NotNull(unboxed);

            unboxed = unboxed.ToList();
            Assert.NotEmpty(unboxed);

            Assert.Equal(_category.Sections[0].Recommendations[0].Page.Slug, unboxed.First().RecommendationSlug);
            Assert.Equal(_category.Sections[0].Recommendations[0].DisplayName, unboxed.First().RecommendationDisplayName);
            Assert.Equal(_categoryTwo.Sections[0].Recommendations[0].Page.Slug, unboxed.Skip(1).First().RecommendationSlug);
            Assert.Equal(_categoryTwo.Sections[0].Recommendations[0].DisplayName, unboxed.Skip(1).First().RecommendationDisplayName);
            Assert.Null(unboxed.First().NoRecommendationFoundErrorMessage);
            Assert.Null(unboxed.Skip(1).First().NoRecommendationFoundErrorMessage);
        }

        [Fact]
        public async Task Returns_RecommendationInfo_And_Logs_Error_If_Exception_Thrown_By_Get_Category()
        {
            _category.SectionStatuses.Add(new Domain.Submissions.Models.SectionStatusDto()
            {
                SectionId = "Section1",
                Completed = 1,
                Maturity = "High"
            });

            Category[] categories = [_category];

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Throws(new Exception("test"));

            var result = await _recommendationsComponent.InvokeAsync(categories) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as IEnumerable<RecommendationsViewComponentViewModel>;
            Assert.NotNull(unboxed);

            unboxed = unboxed.ToList();
            Assert.NotEmpty(unboxed);

            Assert.Equal(_category.Sections[0].Recommendations[0].Page.Slug, unboxed.First().RecommendationSlug);
            Assert.Equal(_category.Sections[0].Recommendations[0].DisplayName, unboxed.First().RecommendationDisplayName);
            Assert.Null(unboxed.First().NoRecommendationFoundErrorMessage);
            _loggerCategory.ReceivedWithAnyArgs(1).LogError("An exception has occurred while trying to retrieve section progress with the following message - test");
        }

        [Fact]
        public async Task Returns_NullRecommendationInfo_If_No_RecommendationPage_Exists_ForMaturity()
        {
            _category.SectionStatuses.Add(new Domain.Submissions.Models.SectionStatusDto()
            {
                SectionId = "Section1",
                Completed = 1,
                Maturity = "Low",
            });

            Category[] categories = new Category[] { _category };

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns(_category.SectionStatuses.ToList());

            var result = await _recommendationsComponent.InvokeAsync(categories) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as IEnumerable<RecommendationsViewComponentViewModel>;
            Assert.NotNull(unboxed);

            unboxed = unboxed.ToList();
            Assert.NotEmpty(unboxed);

            Assert.Null(unboxed.First().RecommendationSlug);
            Assert.Null(unboxed.First().RecommendationDisplayName);
            Assert.NotNull(unboxed.First().NoRecommendationFoundErrorMessage);
        }

        [Fact]
        public async Task DoesNotReturn_RecommendationInfo_If_Section_IsNot_Completed()
        {
            _category.Completed = 0;
            _category.SectionStatuses.Add(new Domain.Submissions.Models.SectionStatusDto()
            {
                SectionId = "Section1",
                Completed = 0,
                Maturity = null
            });

            Category[] categories = [_category];

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns(_category.SectionStatuses.ToList());

            var result = await _recommendationsComponent.InvokeAsync(categories) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.Null(model);
        }

        [Fact]
        public async Task Returns_Null_If_Category_IsNot_Completed()
        {
            _category.Completed = 0;

            Category[] categories = [_category];

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns([.. _category.SectionStatuses]);
            var result = await _recommendationsComponent.InvokeAsync(categories) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.Null(model);
        }
    }
}