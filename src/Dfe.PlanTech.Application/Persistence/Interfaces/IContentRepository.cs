namespace Dfe.PlanTech.Application.Persistence.Interfaces;

/// <summary>
/// Abstraction around repositories used for retrieving Content
/// </summary>
public interface IContentRepository
{
    /// <summary>
    /// Get entity by Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    Task<TEntity?> GetEntityById<TEntity>(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all entities of the specified type.
    /// </summary>
    /// <param name="entityTypeId"></param>
    /// <param name="queries">Additional filtere</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetEntities<TEntity>(string entityTypeId, IGetEntitiesOptions? options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all entities of the specified type, using the name of the generic parameter's type as the entity type id (to lower case).
    /// E.g. if the TEntity is a class called "Category", then it uses "category"
    /// </summary>
    /// <param name="options">Filtering options</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetEntities<TEntity>(IGetEntitiesOptions options, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get entities without filtering
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetEntities<TEntity>(CancellationToken cancellationToken = default);
}
