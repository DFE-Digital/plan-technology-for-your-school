using Dfe.PlanTech.Domain.Content.Models;

namespace Dfe.PlanTech.AzureFunctions.E2ETests.Generators;

public record UpdatedEntity<TEntity>(TEntity Original, TEntity Updated)
where TEntity : ContentComponent
{

}