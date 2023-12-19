using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Dfe.PlanTech.AzureFunctions.Mappings;
using Dfe.PlanTech.AzureFunctions.UnitTests.Mappers;
using Dfe.PlanTech.Domain.Questionnaire.Models;
using Dfe.PlanTech.Infrastructure.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace Dfe.PlanTech.AzureFunctions.UnitTests;

public class QueueReceiverTests
{
    private const string bodyJsonStr = "{\"metadata\":{\"tags\":[]},\"fields\":{\"internalName\":{\"en-US\":\"TestingQuestion\"},\"text\":{\"en-US\":\"TestingQuestion\"},\"helpText\":{\"en-US\":\"HelpText\"},\"answers\":{\"en-US\":[{\"sys\":{\"type\":\"Link\",\"linkType\":\"Entry\",\"id\":\"4QscetbCYG4MUsGdoDU0C3\"}}]},\"slug\":{\"en-US\":\"testing-slug\"}},\"sys\":{\"type\":\"Entry\",\"id\":\"2VSR0emw0SPy8dlR9XlgfF\",\"space\":{\"sys\":{\"type\":\"Link\",\"linkType\":\"Space\",\"id\":\"py5afvqdlxgo\"}},\"environment\":{\"sys\":{\"id\":\"dev\",\"type\":\"Link\",\"linkType\":\"Environment\"}},\"contentType\":{\"sys\":{\"type\":\"Link\",\"linkType\":\"ContentType\",\"id\":\"question\"}},\"createdBy\":{\"sys\":{\"type\":\"Link\",\"linkType\":\"User\",\"id\":\"5yhMQOCN9P2vGpfjyZKiey\"}},\"updatedBy\":{\"sys\":{\"type\":\"Link\",\"linkType\":\"User\",\"id\":\"4hiJvkyVWdhTt6c4ZoDkMf\"}},\"revision\":13,\"createdAt\":\"2023-12-04T14:36:46.614Z\",\"updatedAt\":\"2023-12-15T16:16:45.034Z\"}}";

    private readonly QueueReceiver _queueReceiver;

    private readonly ILoggerFactory _loggerFactoryMock;
    private readonly ILogger _loggerMock;
    private readonly CmsDbContext _cmsDbContextMock;
    private readonly JsonToEntityMappers _jsonToEntityMappers;

    public QueueReceiverTests()
    {
        _loggerFactoryMock = Substitute.For<ILoggerFactory>();
        _loggerMock = Substitute.For<ILogger>();

        _loggerFactoryMock.CreateLogger<Arg.AnyType>().Returns((callinfo) =>
        {
            return _loggerMock;
        });

        _cmsDbContextMock = Substitute.For<CmsDbContext>();

        JsonSerializerOptions jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };

        Type questionDbEntityType = typeof(QuestionDbEntity);

        _jsonToEntityMappers = Substitute.For<JsonToEntityMappers>(new JsonToDbMapper[] { new JsonToDbMapperImplementation(questionDbEntityType, _loggerMock, jsonOptions) }, jsonOptions);

        _queueReceiver = new QueueReceiver(_loggerFactoryMock, _cmsDbContextMock, _jsonToEntityMappers);
    }

    [Fact]
    public async Task QueueReceiverDbWriter_Should_Execute_Successfully()
    {
        _cmsDbContextMock.SaveChangesAsync().Returns(1);

        ServiceBusReceivedMessage serviceBusReceivedMessageMock = Substitute.For<ServiceBusReceivedMessage>();
        ServiceBusMessageActions serviceBusMessageActionsMock = Substitute.For<ServiceBusMessageActions>();

        var subject = "ContentManagement.Entry.create";
        var serviceBusMessage = new ServiceBusMessage(bodyJsonStr) { Subject = subject };

        ServiceBusReceivedMessage serviceBusReceivedMessage = ServiceBusReceivedMessage.FromAmqpMessage(serviceBusMessage.GetRawAmqpMessage(), BinaryData.FromBytes(Encoding.UTF8.GetBytes(serviceBusReceivedMessageMock.LockToken)));

        await _queueReceiver.QueueReceiverDbWriter(new ServiceBusReceivedMessage[] { serviceBusReceivedMessage }, serviceBusMessageActionsMock);

        await serviceBusMessageActionsMock.Received().CompleteMessageAsync(Arg.Any<ServiceBusReceivedMessage>());
    }

    [Fact]
    public async Task QueueReceiverDbWriter_Should_DeadLetter_Failed_Operation()
    {
        ServiceBusReceivedMessage serviceBusReceivedMessageMock = Substitute.For<ServiceBusReceivedMessage>();
        ServiceBusMessageActions serviceBusMessageActionsMock = Substitute.For<ServiceBusMessageActions>();

        string nonMappableJson = "\"INVALID\":\"CONTENT\"";

        var subject = "ContentManagement.Entry.create";
        var serviceBusMessage = new ServiceBusMessage(nonMappableJson) { Subject = subject };

        ServiceBusReceivedMessage serviceBusReceivedMessage = ServiceBusReceivedMessage.FromAmqpMessage(serviceBusMessage.GetRawAmqpMessage(), BinaryData.FromBytes(Encoding.UTF8.GetBytes(serviceBusReceivedMessageMock.LockToken)));

        await _queueReceiver.QueueReceiverDbWriter(new ServiceBusReceivedMessage[] { serviceBusReceivedMessage }, serviceBusMessageActionsMock);

        await serviceBusMessageActionsMock.Received().DeadLetterMessageAsync(Arg.Any<ServiceBusReceivedMessage>());
    }

    [Fact]
    public async Task QueueReceiverDbWriter_Should_MapExistingDbEntity_To_Message()
    {
        ContentComponentDbEntityImplementation contentComponent = new() { Archived = true, Published = true, Deleted = true };

        _cmsDbContextMock.SaveChangesAsync().Returns(1);
        _cmsDbContextMock.Find(Arg.Any<Type>(), Arg.Any<String>()).Returns(contentComponent);

        ServiceBusReceivedMessage serviceBusReceivedMessageMock = Substitute.For<ServiceBusReceivedMessage>();
        ServiceBusMessageActions serviceBusMessageActionsMock = Substitute.For<ServiceBusMessageActions>();

        var subject = "ContentManagement.Entry.save";
        var serviceBusMessage = new ServiceBusMessage(bodyJsonStr) { Subject = subject };

        ServiceBusReceivedMessage serviceBusReceivedMessage = ServiceBusReceivedMessage.FromAmqpMessage(serviceBusMessage.GetRawAmqpMessage(), BinaryData.FromBytes(Encoding.UTF8.GetBytes(serviceBusReceivedMessageMock.LockToken)));

        await _queueReceiver.QueueReceiverDbWriter(new ServiceBusReceivedMessage[] { serviceBusReceivedMessage }, serviceBusMessageActionsMock);

        await serviceBusMessageActionsMock.Received().CompleteMessageAsync(Arg.Any<ServiceBusReceivedMessage>());
    }
}