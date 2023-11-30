using System.Net;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

namespace Dfe.PlanTech.AzureFunctions
{
    public class ContentfulWebHook
    {
        private readonly ILogger _logger;
        private readonly ServiceBusSender _sender;

        public ContentfulWebHook(ILoggerFactory loggerFactory, IAzureClientFactory<ServiceBusSender> serviceBusSenderFactory)
        {
            _logger = loggerFactory.CreateLogger<ContentfulWebHook>();
            _sender = serviceBusSenderFactory.CreateClient("contentful");
        }

        [Function("ContentfulWebHook")]
        public async Task<HttpResponseData> WebhookReceiver([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Received webhook POST.");

            var stream = new StreamReader(req.Body);
            var body = stream.ReadToEnd();

            if (string.IsNullOrEmpty(body))
            {
                return ReturnEmptyBodyError(req);
            }

            _logger.LogTrace("Logging message body: {body}", body);

            try
            {
                await WriteToQueue(body);

                return ReturnOkResponse(req);
            }
            catch (Exception ex)
            {
                return ReturnServerErrorResponse(req, ex);
            }
        }

        private HttpResponseData ReturnServerErrorResponse(HttpRequestData req, Exception ex)
        {
            _logger.LogError("Error writing body to queue - {message} {stacktrace}", ex.Message, ex.StackTrace);
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }

        private static HttpResponseData ReturnOkResponse(HttpRequestData req) => req.CreateResponse(HttpStatusCode.OK);

        private async Task WriteToQueue(string body)
        {
            var serviceBusMessage = new ServiceBusMessage(body);
            await _sender.SendMessageAsync(serviceBusMessage);
        }

        private HttpResponseData ReturnEmptyBodyError(HttpRequestData req)
        {
            _logger.LogError("Received null body.");
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
