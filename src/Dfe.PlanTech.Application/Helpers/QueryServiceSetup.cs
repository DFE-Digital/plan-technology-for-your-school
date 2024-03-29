using Dfe.PlanTech.Application.Content.Queries;
using Dfe.PlanTech.Application.Persistence.Interfaces;
using Dfe.PlanTech.Domain.Content.Interfaces;
using Dfe.PlanTech.Domain.Content.Queries;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Dfe.PlanTech.Application.Helpers;

/// <summary>
/// Adds CQRS classes to application
/// </summary>
public static class QueryServiceSetup
{
    /// <summary>
    /// Adds CQRS commands/queries as services
    /// </summary>
    /// <param name="services">IServiceCollection from dependency injection</param>
    /// <returns>Services (so methods can be chained easily)</returns>
    public static IServiceCollection AddCQRSServices(this IServiceCollection services)
    {
        var queries = Assembly.GetExecutingAssembly().GetTypes().Where(IsConcreteQueryClass);

        foreach (var query in queries)
        {
            services.AddScoped(query, query);
        }

        services.AddScoped<IGetPageQuery, GetPageQuery>();
        services.AddScoped<IGetPageChildrenQuery, GetButtonWithEntryReferencesQuery>();
        services.AddScoped<IGetPageChildrenQuery, GetCategorySectionsQuery>();
        services.AddScoped<IGetPageChildrenQuery, GetRichTextsForPageQuery>();

        return services;
    }

    /// <summary>
    /// Checks if the given type is concrete (not abstract + not interface), and inherits <see href="IInfrastructureQuery"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsConcreteQueryClass(Type type)
    => !type.IsAbstract && !type.IsInterface && type.GetInterface(nameof(IInfrastructureQuery)) != null;
}
