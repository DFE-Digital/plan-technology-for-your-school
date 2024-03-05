using Azure.Messaging.ServiceBus;
using Dfe.PlanTech.AzureFunctions.Mappings;
using Dfe.PlanTech.AzureFunctions.Models;
using Dfe.PlanTech.Domain.Caching.Enums;
using Dfe.PlanTech.Domain.Caching.Exceptions;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Domain.Persistence.Models;
using Dfe.PlanTech.Infrastructure.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Dfe.PlanTech.AzureFunctions;

public class QueueReceiver : BaseFunction
{
    private readonly CmsDbContext _db;
    private readonly ContentfulOptions _contentfulOptions = new(true);
    private readonly JsonToEntityMappers _mappers;

    public QueueReceiver(ContentfulOptions contentfulOptions, ILoggerFactory loggerFactory, CmsDbContext db, JsonToEntityMappers mappers) : base(loggerFactory.CreateLogger<QueueReceiver>())
    {
        _contentfulOptions = contentfulOptions;
        _db = db;
        _mappers = mappers;
    }

    /// <summary>
    /// Azure Function App function that processes messages from a Service Bus queue, converts them
    /// to the appropriate <see cref="ContentComponentDbEntity"/> class, and adds/updates the database where appropriate.
    /// </summary>
    /// <param name="messages">Array of ServiceBusReceivedMessage objects representing the messages to be processed</param>
    /// <param name="messageActions">object providing actions for handling the messages.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("QueueReceiver")]
    public async Task QueueReceiverDbWriter([ServiceBusTrigger("contentful", IsBatched = true)] ServiceBusReceivedMessage[] messages, ServiceBusMessageActions messageActions, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Queue Receiver -> Db Writer started. Processing {msgCount} messages", messages.Length);

        foreach (ServiceBusReceivedMessage message in messages)
        {
            await ProcessMessage(message, messageActions, cancellationToken);
        }
    }

    /// <summary>
    /// Processes a message from the Service Bus, updating the corresponding database entities,
    /// and completing or dead-lettering the message. 
    /// </summary>
    /// <param name="message">Service bus message to process</param>
    /// <param name="messageActions">Service bus message actions for completing or dead-lettering</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ProcessMessage(ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, CancellationToken cancellationToken)
    {
        try
        {
            CmsEvent cmsEvent = GetCmsEvent(message.Subject);

            if (ShouldIgnoreMessage(cmsEvent))
            {
                await messageActions.CompleteMessageAsync(message, cancellationToken);
                return;
            }

            MappedEntity mapped = await MapMessageToEntity(message, cmsEvent, cancellationToken);

            if (!mapped.IsValid)
            {
                await messageActions.CompleteMessageAsync(message, cancellationToken);
                return;
            }

            UpsertEntity(cmsEvent, mapped);

            await DbSaveChanges(cancellationToken);

            await messageActions.CompleteMessageAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message);
            await messageActions.DeadLetterMessageAsync(message, null, ex.Message, ex.StackTrace, cancellationToken);
        }
    }

    /// <summary>
    /// Checks if the message with the given CmsEvent should be ignored based on certain conditions.
    /// </summary>
    /// <remarks>
    /// If we recieve a create event, we return true.
    /// If we are NOT using preview mode (i.e. we are ignoring drafts), and the event is just a save or autosave, then return true.
    /// 
    /// Else, we return false.
    /// </remarks>
    /// <param name="cmsEvent"></param>
    /// <returns></returns>
    private bool ShouldIgnoreMessage(CmsEvent cmsEvent)
    {
        if (cmsEvent == CmsEvent.CREATE)
        {
            Logger.LogInformation("Dropping received event {cmsEvent}", cmsEvent);
            return true;
        }

        if ((cmsEvent == CmsEvent.SAVE || cmsEvent == CmsEvent.AUTO_SAVE) && !_contentfulOptions.UsePreview)
        {
            Logger.LogInformation("Receieved {event} but UsePreview is {usePreview} - dropping message", cmsEvent, _contentfulOptions.UsePreview);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Retrieves the CmsEvent based on the provided subject. 
    /// </summary>
    /// <param name="contentfulEvent">The Contentful event (from the service bus message subject)</param>
    /// <returns></returns>
    /// <exception cref="CmsEventException">Exception thrown if we are unable to parse the event to a valid <see cref="CmsEvent"/></exception>
    private CmsEvent GetCmsEvent(string contentfulEvent)
    {
        if (!Enum.TryParse(contentfulEvent.AsSpan()[(contentfulEvent.LastIndexOf('.') + 1)..], true, out CmsEvent cmsEvent))
        {
            throw new CmsEventException(string.Format("Cannot parse header \"{0}\" into a valid CMS event", contentfulEvent));
        }

        Logger.LogInformation("CMS Event: {cmsEvent}", cmsEvent);

        return cmsEvent;
    }

    /// <summary>
    /// Maps the incoming message to the appropriate <see cref="ContentComponentDbEntity"/>
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>

    private Task<MappedEntity> MapMessageToEntity(ServiceBusReceivedMessage message, CmsEvent cmsEvent, CancellationToken cancellationToken)
    {
        string messageBody = Encoding.UTF8.GetString(message.Body);

        Logger.LogInformation("Processing = {messageBody}", messageBody);

        return _mappers.ToEntity(messageBody, cmsEvent, cancellationToken);
    }

    /// <summary>
    /// Either add the entity to DB, or update the properties of the existing entity
    /// </summary>
    /// <param name="cmsEvent"></param>
    /// <param name="mapped"></param>
    /// <param name="existing"></param>
    private void UpsertEntity(CmsEvent cmsEvent, MappedEntity mappedEntity)
    {
        if (cmsEvent == CmsEvent.UNPUBLISH) return;

        if (!mappedEntity.AlreadyExistsInDatabase)
        {
            _db.Add(mappedEntity.IncomingEntity);
        }
    }

    /// <summary>
    /// Saves changes in database, and logs information about rows changed
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task DbSaveChanges(CancellationToken cancellationToken)
    {
        long rowsChangedInDatabase = await _db.SaveChangesAsync(cancellationToken);

        if (rowsChangedInDatabase == 0L)
        {
            Logger.LogWarning("No rows changed in the database!");
        }
        else
        {
            Logger.LogInformation($"Updated {rowsChangedInDatabase} rows in the database");
        }
    }
}