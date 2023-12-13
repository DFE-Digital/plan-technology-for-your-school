using System.Net;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Dfe.PlanTech.AzureFunctions.UnitTests;

public class ContentfulWebHookTests
{
  private readonly ContentfulWebHook _contentfulWebHook;

  //Mocks
  private readonly ILoggerFactory _loggerFactory;
  private readonly ILogger _logger;
  private readonly IAzureClientFactory<ServiceBusSender> _serviceBusFactory;
  private readonly ServiceBusSender _serviceBusSender;

  public ContentfulWebHookTests()
  {
    _loggerFactory = Substitute.For<ILoggerFactory>();
    _logger = Substitute.For<ILogger>();

    _loggerFactory.CreateLogger<Arg.AnyType>().Returns((callinfo) =>
    {
      return _logger;
    });

    _serviceBusSender = Substitute.For<ServiceBusSender>();

    _serviceBusFactory = Substitute.For<IAzureClientFactory<ServiceBusSender>>();

    _serviceBusFactory.CreateClient(Arg.Any<string>()).Returns((callInfo) =>
    {
      return _serviceBusSender;
    });

    _contentfulWebHook = new ContentfulWebHook(_loggerFactory, _serviceBusFactory);
  }

  public static Stream GenerateStreamFromString(string? s)
  {
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write(s);
    writer.Flush();
    stream.Position = 0;
    return stream;
  }

  [Fact]
  public async Task WebhookReceiver_Should_Error_If_NullOrEmptyBody()
  {
    var request = HttpHelpers.MockHttpRequest();

    var stream = GenerateStreamFromString(null);
    request.Body.Returns(stream);

    var result = await _contentfulWebHook.WebhookReceiver(request);

    Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task WebhookReceiver_Should_ReturnOk_When_BodyNotNull()
  {
    ServiceBusMessage? serviceBusMessage = null;

    _serviceBusSender.SendMessageAsync(Arg.Any<ServiceBusMessage>(), Arg.Any<CancellationToken>())
                      .Returns(callinfo =>
                      {
                        serviceBusMessage = callinfo.ArgAt<ServiceBusMessage>(0);
                        return Task.CompletedTask;
                      });

    var request = HttpHelpers.MockHttpRequest();

    var requestBody = "request body";
    var stream = GenerateStreamFromString(requestBody);
    request.Body.Returns(stream);

    var result = await _contentfulWebHook.WebhookReceiver(request);

    Assert.True(result.StatusCode == HttpStatusCode.OK);

    Assert.NotNull(serviceBusMessage);

    var messageBody = serviceBusMessage.Body.ToString();

    Assert.Equal(requestBody, messageBody);
  }

  [Fact]
  public async Task WebhookReceiver_Should_ReturnError_When_ExceptionThrown()
  {
    var errorMessage = "ERROR MESSAGE";

    _serviceBusSender.SendMessageAsync(Arg.Any<ServiceBusMessage>(), Arg.Any<CancellationToken>())
                      .Returns(callinfo =>
                      {
                        throw new Exception(errorMessage);
                      });

    var request = HttpHelpers.MockHttpRequest();

    var requestBody = "request body";
    var stream = GenerateStreamFromString(requestBody);
    request.Body.Returns(stream);

    var result = await _contentfulWebHook.WebhookReceiver(request);

    Assert.True(result.StatusCode == HttpStatusCode.InternalServerError);
  }
}