using Dfe.PlanTech.AzureFunctions.Models;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Dfe.PlanTech.AzureFunctions.Mappings;

public class PageEntityUpdater(ILogger<PageEntityUpdater> logger, CmsDbContext db) : EntityUpdater(logger, db)
{
    public override MappedEntity UpdateEntityConcrete(MappedEntity entity)
    {
        if (!entity.AlreadyExistsInDatabase)
        {
            return entity;
        }

        if (entity.IncomingEntity is not PageDbEntity incomingPage || entity.ExistingEntity is not PageDbEntity existingPage)
        {
            throw new InvalidCastException($"Entities are not expected page types. Received {entity.IncomingEntity.GetType()} and {entity.ExistingEntity!.GetType()}");
        }

        ProcessPageContentChanges(incomingPage, existingPage);

        return entity;
    }

    /// <summary>
    /// CRUD page contents for the page
    /// </summary>
    /// <param name="incomingPage"></param>
    /// <param name="existingPage"></param>
    private static void ProcessPageContentChanges(PageDbEntity incomingPage, PageDbEntity existingPage)
    {
        AddOrUpdatePageContents(incomingPage, existingPage);
        DeleteRemovedPageContents(incomingPage, existingPage);
    }

    /// <summary>
    /// Removes page contents from existing page if they are not in the incoming page update
    /// </summary>
    /// <param name="incomingPage"></param>
    /// <param name="existingPage"></param>
    private static void DeleteRemovedPageContents(PageDbEntity incomingPage, PageDbEntity existingPage)
    {
        var deletedPageContents = existingPage.AllPageContents.Where((existingPageContent) => !HasPageContent(incomingPage, existingPageContent)).ToList();

        existingPage.AllPageContents.RemoveAll((existingPageContent) => !HasPageContent(incomingPage, existingPageContent));
    }

    private static bool HasPageContent(PageDbEntity page, PageContentDbEntity pageContent)
     => page.AllPageContents.Any(incomingPageContent => incomingPageContent.Matches(pageContent));

    /// <summary>
    /// For each incoming page content:
    /// - Add if it doesn't already exist
    /// - Remove duplicates if any
    /// - Update order if existing and order has changed
    /// </summary>
    /// <param name="incomingPage"></param>
    /// <param name="existingPage"></param>
    private static void AddOrUpdatePageContents(PageDbEntity incomingPage, PageDbEntity existingPage)
    {
        foreach (var pageContent in incomingPage.AllPageContents)
        {
            var matchingContents = existingPage.AllPageContents.Where(pc => pc.Matches(pageContent))
                                                              .OrderByDescending(pc => pc.Id)
                                                              .ToList();

            ProcessPageContent(existingPage, pageContent, matchingContents);
        }
    }

    /// <summary>
    /// - Add page content if missing from existing page
    /// - Remove duplicates from existing page if found
    /// - Update order if changed
    /// </summary>
    /// <param name="existingPage"></param>
    /// <param name="pageContent"></param>
    /// <param name="matchingContents"></param>
    private static void ProcessPageContent(PageDbEntity existingPage, PageContentDbEntity pageContent, List<PageContentDbEntity> matchingContents)
    {
        if (matchingContents.Count == 0)
        {
            existingPage.AllPageContents.Add(pageContent);
            return;
        }

        RemoveDuplicates(existingPage, matchingContents);
        UpdatePageContentOrder(pageContent, matchingContents);
    }

    /// <summary>
    /// Updates the order of the page content if it's different
    /// </summary>
    /// <param name="pageContent"></param>
    /// <param name="matchingContents"></param>
    private static void UpdatePageContentOrder(PageContentDbEntity pageContent, List<PageContentDbEntity> matchingContents)
    {
        var remainingMatchingContent = matchingContents[0];

        //Only change the order if it has actually changed, to prevent unnecessary updates to the DB by EF Core
        if (remainingMatchingContent.Order != pageContent.Order)
        {
            remainingMatchingContent.Order = pageContent.Order;
        }
    }

    /// <summary>
    /// Remove duplicate page content entities
    /// </summary>
    /// <remarks>
    /// Shouldn't be needed anymore, but exists to fix old data issues if they arise.
    /// </remarks>
    /// <param name="existingPage"></param>
    /// <param name="matchingContents"></param>
    private static void RemoveDuplicates(PageDbEntity existingPage, List<PageContentDbEntity> matchingContents)
    {
        if (matchingContents.Count > 1)
        {
            existingPage.AllPageContents.RemoveAll((pc) => matchingContents[1..].Contains(pc));
        }
    }
}
