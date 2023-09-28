using Dfe.PlanTech.Domain.CategorySection;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Domain.Interfaces;
using Dfe.PlanTech.Domain.Questionnaire.Interfaces;
using Dfe.PlanTech.Domain.Questionnaire.Models;
using Dfe.PlanTech.Domain.Submissions.Models;
using Dfe.PlanTech.Web.Models;
using Dfe.PlanTech.Web.ViewComponents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Dfe.PlanTech.Web.UnitTests.ViewComponents
{
    public class CategorySectionViewComponentTests
    {
        private readonly IGetSubmissionStatusesQuery _getSubmissionStatusesQuery;
        private readonly CategorySectionViewComponent _categorySectionViewComponent;

        private ICategory _category;
        private ILogger<CategorySectionViewComponent> _loggerCategory;

        public CategorySectionViewComponentTests()
        {
            _getSubmissionStatusesQuery = Substitute.For<IGetSubmissionStatusesQuery>();
            _loggerCategory = Substitute.For<ILogger<CategorySectionViewComponent>>();

            var viewContext = new ViewContext();

            var viewComponentContext = new ViewComponentContext
            {
                ViewContext = viewContext
            };

            _categorySectionViewComponent = new CategorySectionViewComponent(_loggerCategory, _getSubmissionStatusesQuery)
            {
                ViewComponentContext = viewComponentContext
            };

            _category = new Category()
            {
                Completed = 1,
                Sections = new Section[]
                {
                    new ()
                    {
                        Sys = new SystemDetails() { Id = "Section1" },
                        Name = "Test Section 1",
                        InterstitialPage = new Page()
                        {
                            Slug = "section-1",
                        }
                    }
                }
            };
        }

        [Fact]
        public void Returns_CategorySectionInfo_If_Slug_Exists_And_SectionIsCompleted()
        {
            _category.SectionStatuses.Add(new Domain.Submissions.Models.SectionStatus()
            {
                SectionId = "Section1",
                Completed = 0,
            });

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns(_category.SectionStatuses);

            var result = _categorySectionViewComponent.Invoke(_category) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as CategorySectionViewComponentViewModel;
            Assert.NotNull(unboxed);

            Assert.Equal(1, unboxed.CompletedSectionCount);
            Assert.Equal(1, unboxed.TotalSectionCount);

            var categorySectionDtoList = unboxed.CategorySectionDto as IEnumerable<CategorySectionDto>;

            Assert.NotNull(categorySectionDtoList);

            categorySectionDtoList = categorySectionDtoList.ToList();
            Assert.NotEmpty(categorySectionDtoList);

            var categorySectionDto = categorySectionDtoList.FirstOrDefault();

            Assert.NotNull(categorySectionDto);

            Assert.Equal("section-1", categorySectionDto.Slug);
            Assert.Equal("Test Section 1", categorySectionDto.Name);
            Assert.Equal("DarkBlue", categorySectionDto.TagColour);
            Assert.Equal("COMPLETE", categorySectionDto.TagText);
            Assert.Null(categorySectionDto.NoSlugForSubtopicErrorMessage);
        }

        [Fact]
        public void Returns_CategorySelectionInfo_If_Slug_Exists_And_SectionIsNotCompleted()
        {
            _category.Completed = 0;

            _category.SectionStatuses.Add(new SectionStatus()
            {
                SectionId = "Section1",
                Completed = 0,
            });

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns(_category.SectionStatuses);

            var result = _categorySectionViewComponent.Invoke(_category) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as CategorySectionViewComponentViewModel;
            Assert.NotNull(unboxed);

            Assert.Equal(0, unboxed.CompletedSectionCount);
            Assert.Equal(1, unboxed.TotalSectionCount);

            var categorySectionDtoList = unboxed.CategorySectionDto as IEnumerable<CategorySectionDto>;

            Assert.NotNull(categorySectionDtoList);

            categorySectionDtoList = categorySectionDtoList.ToList();
            Assert.NotEmpty(categorySectionDtoList);

            var categorySectionDto = categorySectionDtoList.FirstOrDefault();

            Assert.NotNull(categorySectionDto);

            Assert.Equal("section-1", categorySectionDto.Slug);
            Assert.Equal("Test Section 1", categorySectionDto.Name);
            Assert.Equal("Blue", categorySectionDto.TagColour);
            Assert.Equal("IN PROGRESS", categorySectionDto.TagText);
            Assert.Null(categorySectionDto.NoSlugForSubtopicErrorMessage);
        }

        [Fact]
        public void Returns_CategorySelectionInfo_If_Section_IsNotStarted()
        {
            _category.Completed = 0;

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns(_category.SectionStatuses);

            var result = _categorySectionViewComponent.Invoke(_category) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as CategorySectionViewComponentViewModel;
            Assert.NotNull(unboxed);

            Assert.Equal(0, unboxed.CompletedSectionCount);
            Assert.Equal(1, unboxed.TotalSectionCount);

            var categorySectionDtoList = unboxed.CategorySectionDto as IEnumerable<CategorySectionDto>;

            Assert.NotNull(categorySectionDtoList);

            categorySectionDtoList = categorySectionDtoList.ToList();
            Assert.NotEmpty(categorySectionDtoList);

            var categorySectionDto = categorySectionDtoList.FirstOrDefault();

            Assert.NotNull(categorySectionDto);

            Assert.Equal("section-1", categorySectionDto.Slug);
            Assert.Equal("Test Section 1", categorySectionDto.Name);
            Assert.Equal("Grey", categorySectionDto.TagColour);
            Assert.Equal("NOT STARTED", categorySectionDto.TagText);
            Assert.Null(categorySectionDto.NoSlugForSubtopicErrorMessage);
        }

        [Fact]
        public void Returns_NullSlug_And_ErrorMessage_In_CategorySectionInfo_If_SlugDoesNotExist()
        {
            _category.Sections[0] = new Section()
            {
                Sys = new SystemDetails() { Id = "Section1" },
                Name = "Test Section 1",
                InterstitialPage = new Page()
                {
                    Slug = null!,
                }
            };

            _category.SectionStatuses.Add(new SectionStatus()
            {
                SectionId = "Section1",
                Completed = 1,
            });

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns(_category.SectionStatuses);

            var result = _categorySectionViewComponent.Invoke(_category) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as CategorySectionViewComponentViewModel;
            Assert.NotNull(unboxed);

            Assert.Equal(1, unboxed.CompletedSectionCount);
            Assert.Equal(1, unboxed.TotalSectionCount);

            var categorySectionDtoList = unboxed.CategorySectionDto as IEnumerable<CategorySectionDto>;

            Assert.NotNull(categorySectionDtoList);

            categorySectionDtoList = categorySectionDtoList.ToList();
            Assert.NotEmpty(categorySectionDtoList);

            var categorySectionDto = categorySectionDtoList.FirstOrDefault();

            Assert.NotNull(categorySectionDto);

            Assert.Null(categorySectionDto.Slug);
            Assert.Equal("Test Section 1", categorySectionDto.Name);
            Assert.Null(categorySectionDto.TagColour);
            Assert.Null(categorySectionDto.TagText);
            Assert.NotNull(categorySectionDto.NoSlugForSubtopicErrorMessage);
            Assert.Equal("Test Section 1 unavailable", categorySectionDto.NoSlugForSubtopicErrorMessage);
        }

        [Fact]
        public void Returns_ProgressRetrievalError_When_ProgressCanNotBeRetrieved()
        {
            _category.SectionStatuses = null!;

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns(_category.SectionStatuses);

            var result = _categorySectionViewComponent.Invoke(_category) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as CategorySectionViewComponentViewModel;
            Assert.NotNull(unboxed);
            Assert.Equal("Unable to retrieve progress, please refresh your browser.", unboxed.ProgressRetrievalErrorMessage);

            var categorySectionDtoList = unboxed.CategorySectionDto as IEnumerable<CategorySectionDto>;

            Assert.NotNull(categorySectionDtoList);

            categorySectionDtoList = categorySectionDtoList.ToList();
            Assert.NotEmpty(categorySectionDtoList);

            var categorySectionDto = categorySectionDtoList.FirstOrDefault();

            Assert.NotNull(categorySectionDto);

            Assert.Equal("section-1", categorySectionDto.Slug);
            Assert.Equal("Test Section 1", categorySectionDto.Name);
            Assert.Equal("Red", categorySectionDto.TagColour);
            Assert.Equal("UNABLE TO RETRIEVE STATUS", categorySectionDto.TagText);
            Assert.Null(categorySectionDto.NoSlugForSubtopicErrorMessage);
        }

        [Fact]
        public void Returns_NoSectionsErrorRedirectUrl_If_SectionsAreNull()
        {
            _category = new Category()
            {
                Completed = 0,
                Sections = null!
            };

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns(_category.SectionStatuses);

            var result = _categorySectionViewComponent.Invoke(_category) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as CategorySectionViewComponentViewModel;
            Assert.NotNull(unboxed);
            Assert.Equal("ServiceUnavailable", unboxed.NoSectionsErrorRedirectUrl);
            Assert.Equal(0, unboxed.TotalSectionCount);

            var categorySectionDtoList = unboxed.CategorySectionDto as IEnumerable<CategorySectionDto>;

            Assert.NotNull(categorySectionDtoList);

            categorySectionDtoList = categorySectionDtoList.ToList();
            Assert.Empty(categorySectionDtoList);
        }

        [Fact]
        public void Returns_NoSectionsErrorRedirectUrl_If_SectionsAreEmpty()
        {
            _category = new Category()
            {
                Completed = 0,
                Sections = Array.Empty<ISection>()
            };

            _getSubmissionStatusesQuery.GetSectionSubmissionStatuses(_category.Sections).Returns(_category.SectionStatuses);

            var result = _categorySectionViewComponent.Invoke(_category) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.NotNull(result.ViewData);

            var model = result.ViewData.Model;
            Assert.NotNull(model);

            var unboxed = model as CategorySectionViewComponentViewModel;
            Assert.NotNull(unboxed);
            Assert.Equal("ServiceUnavailable", unboxed.NoSectionsErrorRedirectUrl);
            Assert.Equal(0, unboxed.TotalSectionCount);

            var categorySectionDtoList = unboxed.CategorySectionDto as IEnumerable<CategorySectionDto>;

            Assert.NotNull(categorySectionDtoList);

            categorySectionDtoList = categorySectionDtoList.ToList();
            Assert.Empty(categorySectionDtoList);
        }
    }
}