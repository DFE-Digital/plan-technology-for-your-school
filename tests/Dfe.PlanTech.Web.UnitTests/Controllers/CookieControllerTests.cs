﻿using Dfe.PlanTech.Application.Caching.Interfaces;
using Dfe.PlanTech.Application.Content.Queries;
using Dfe.PlanTech.Application.Cookie.Interfaces;
using Dfe.PlanTech.Application.Persistence.Interfaces;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Infrastructure.Application.Models;
using Dfe.PlanTech.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using NSubstitute;
using Xunit;

namespace Dfe.PlanTech.Web.UnitTests.Controllers
{
    public class CookieControllerTests
    {
        private readonly Page[] _pages = new Page[]
        {
            new Page()
            {
                Slug = "cookies",
                Title = new Title() { Text = "Cookies" },
                Content = new ContentComponent[] { new Header() { Tag = Domain.Content.Enums.HeaderTag.H1, Text = "Analytical Cookies" }}
            },
        };

        public static CookiesController CreateStrut()
        {
            ILogger<CookiesController> loggerMock = Substitute.For<ILogger<CookiesController>>();
            ICookieService cookiesMock = Substitute.For<ICookieService>();

            return new CookiesController(loggerMock, cookiesMock);
        }

        [Theory]
        [InlineData("https://localhost:8080/self-assessment")]
        [InlineData("https://www.dfe.gov.uk/self-assessment")]
        public void HideBanner_Redirects_BackToPlaceOfOrigin(string url)
        {
            //Arrange
            var strut = CreateStrut();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referer"] = url;

            strut.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = strut.HideBanner() as RedirectResult;

            //Assert
            Assert.IsType<RedirectResult>(result);
            Assert.Equal(url, result.Url);
        }

        [Theory]
        [InlineData("https://localhost:8080/self-assessment")]
        [InlineData("https://www.dfe.gov.uk/self-assessment")]
        public void Accept_Redirects_BackToPlaceOfOrigin(string url)
        {
            //Arrange
            var strut = CreateStrut();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referer"] = url;
            strut.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = strut.Accept() as RedirectResult;

            //Assert
            Assert.IsType<RedirectResult>(result);
            Assert.Equal(url, result.Url);
        }

        [Theory]
        [InlineData("https://localhost:8080/self-assessment")]
        [InlineData("https://www.dfe.gov.uk/self-assessment")]
        public void Reject_Redirects_BackToPlaceOfOrigin(string url)
        {
            //Arrange
            var strut = CreateStrut();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referer"] = url;
            strut.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = strut.Reject() as RedirectResult;

            //Assert
            Assert.IsType<RedirectResult>(result);
            Assert.Equal(url, result.Url);
        }

        private static IRequestCookieCollection MockRequestCookieCollection(string key, string value)
        {
            var requestFeature = new HttpRequestFeature();
            var featureCollection = new FeatureCollection();

            requestFeature.Headers = new HeaderDictionary();
            requestFeature.Headers.Add(HeaderNames.Cookie, new StringValues(key + "=" + value));

            featureCollection.Set<IHttpRequestFeature>(requestFeature);

            var cookiesFeature = new RequestCookiesFeature(featureCollection);

            return cookiesFeature.Cookies;
        }
        
        
         [Fact]
         public async Task CookiesPageDisplays()
         {
            IQuestionnaireCacher questionnaireCacherMock = Substitute.For<IQuestionnaireCacher>();
            IContentRepository contentRepositoryMock = SetupRepositoryMock();
            GetPageQuery _getPageQueryMock = Substitute.For<GetPageQuery>(questionnaireCacherMock, contentRepositoryMock);

            CookiesController cookiesController = CreateStrut();
            var result = await cookiesController.GetCookiesPage(_getPageQueryMock);
            Assert.IsType<ViewResult>(result);

            var viewResult = result as ViewResult;

            Assert.NotNull(viewResult);
            Assert.Equal("Cookies", viewResult.ViewName);
        }
         
         [Theory]
         [InlineData("true")]
         [InlineData("false")]
         public void settingCookiePreferenceBasedOnInputRedirectsToCookiePage(string userPreference)
         {
            CookiesController cookiesController = CreateStrut();
             
            var tempDataMock = Substitute.For<ITempDataDictionary>();
            var httpContextMock = Substitute.For<HttpContext>();
            var responseMock = Substitute.For<HttpResponse>();
            var cookiesMock = Substitute.For<ICookieService>();


            cookiesMock.SetPreference(Arg.Any<bool>());
            httpContextMock.Response.Returns(responseMock);

            cookiesController.TempData = tempDataMock;
            cookiesController.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock
            };

            var result = cookiesController.CookiePreference(userPreference);

            tempDataMock.Received(1)["UserPreferenceRecorded"] = true;

             Assert.IsType<RedirectToActionResult>(result);

             var res = result as RedirectToActionResult;

             if (res != null)
             {
                 Assert.True(res.ActionName == "GetByRoute");
                 Assert.True(res.ControllerName == "Pages");
             }
         }

        [Fact]
        public void settingCookiePreferenceBasedOnInputAsNullThrowsException()
        {
            CookiesController cookiesController = CreateStrut();

            var tempDataMock = Substitute.For<ITempDataDictionary>();
            var httpContextMock = Substitute.For<HttpContext>();
            var responseMock = Substitute.For<HttpResponse>();
            var cookiesMock = Substitute.For<ICookieService>();


            cookiesMock.SetPreference(Arg.Any<bool>());
            httpContextMock.Response.Returns(responseMock);

            cookiesController.TempData = tempDataMock;
            cookiesController.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock
            };

            var result = Assert.Throws<ArgumentException>(() => cookiesController.CookiePreference(string.Empty));
            Assert.Contains("Can't convert preference", result.Message);
        }

        private IContentRepository SetupRepositoryMock()
         {
             var repositoryMock = Substitute.For<IContentRepository>();
             repositoryMock.GetEntities<Page>(Arg.Any<IGetEntitiesOptions>(), Arg.Any<CancellationToken>()).Returns((CallInfo) =>
             {
                 IGetEntitiesOptions options = (IGetEntitiesOptions)CallInfo[0];
                 if (options?.Queries != null)
                 {
                     foreach (var query in options.Queries)
                     {
                         if (query is ContentQueryEquals equalsQuery && query.Field == "fields.slug")
                         {
                             return _pages.Where(page => page.Slug == equalsQuery.Value);
                         }
                     }
                 }
                 return Array.Empty<Page>();
             });
             return repositoryMock;
         }
    }
}
