using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AutoMapper;
using Dfe.PlanTech.Domain.Caching.Models;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Domain.Questionnaire.Models;
using Dfe.PlanTech.Infrastructure.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Dfe.PlanTech.AzureFunctions
{
  public class TestingJsonParser
  {
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    private readonly List<Type> _allTypes;

    private readonly CmsDbContext _db;


    public TestingJsonParser(ILoggerFactory loggerFactory, IMapper mapper, CmsDbContext db)
    {
      _logger = loggerFactory.CreateLogger<TestingJsonParser>();
      _mapper = mapper;

      _allTypes = GetAllTypes();
      _db = db;
    }

    private static List<Type> GetAllTypes() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembley => assembley.GetTypes()).ToList();

    [Function("TestingSerialisation")]
    public async Task<HttpResponseData> TestingSerialisation([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
      var stream = new StreamReader(req.Body);
      var body = stream.ReadToEnd();

      try
      {
        var processed = JsonSerializer.Deserialize<CmsWebHookPayload>(body, new JsonSerializerOptions()
        {
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var responseType = HttpStatusCode.OK;

        var response = req.CreateResponse(responseType);

        return response;
      }
      catch (Exception ex)
      {
        var response = req.CreateResponse(HttpStatusCode.InternalServerError);

        var sb = new StringBuilder();

        var currentException = ex;

        while (currentException != null)
        {
          sb.AppendLine(currentException.Message);
          sb.AppendLine(currentException.StackTrace);
          currentException = ex.InnerException;
        }
        await response.WriteStringAsync(sb.ToString());

        return response;
      }
    }

    [Function("TestingJsonParser")]
    public async Task<HttpResponseData> WebhookReceiver([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
      var stream = new StreamReader(req.Body);
      var body = stream.ReadToEnd();

      try
      {
        var processed = TryNormaliseJson(body);

        var responseType = processed ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;

        var response = req.CreateResponse(responseType);

        return response;
      }
      catch (Exception ex)
      {
        var response = req.CreateResponse(HttpStatusCode.InternalServerError);

        var sb = new StringBuilder();

        var currentException = ex;

        while (currentException != null)
        {
          sb.AppendLine(currentException.Message);
          sb.AppendLine(currentException.StackTrace);
          currentException = ex.InnerException;
        }
        await response.WriteStringAsync(sb.ToString());

        return response;
      }
    }

    private bool TryNormaliseJson(string body)
    {
      var success = SerialiseBody(body, out CmsWebHookPayload? payload);

      if (!success) return false;

      var fields = new Dictionary<string, object>();

      foreach (var field in payload!.Fields)
      {
        if (field.Value == null)
        {
          _logger.LogError("No value for {field}", field);
          continue;
        }

        var fieldChildren = field.Value.AsObject();

        if (fieldChildren.Count > 1)
        {
          _logger.LogError("Expected only one language - received {count}", fieldChildren.Count);
          continue;
        }

        foreach (var child in fieldChildren)
        {
          fields[FirstCharToUpperAsSpan(field.Key)] = child.Value;
        }
      }

      fields["Id"] = payload.Sys.Id;

      var contentType = $"{FirstCharToUpperAsSpan(payload.Sys.ContentType.Sys.Id)}DbEntity";
      fields["ContentType"] = contentType;

      var serialised = JsonSerializer.Serialize(fields);

      return true;
      /*
            object mapped = MapObjectToDbEntity(jsonNode, contentType);

            if (mapped is ContentComponentDbEntity contentComponent)
            {
              var existing = _db.Find(mapped.GetType(), contentComponent.Id);

              if (existing != null)
              {
                var properties = mapped.GetType().GetProperties();

                foreach (var property in properties)
                {
                  property.SetValue(existing, property.GetValue(mapped));
                }
              }
              else
              {
                _db.Add(mapped);
              }

              _db.SaveChanges();
            }

            return true;
            */
    }

    private static bool SerialiseBody(string body, out CmsWebHookPayload? payload)
    {
      try
      {
        payload = JsonSerializer.Deserialize<CmsWebHookPayload>(body, new JsonSerializerOptions()
        {
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return payload != null;
      }
      catch (Exception ex)
      {
        throw new Exception($"Error serialising body to {typeof(CmsWebHookPayload)}", ex);
      }
    }

    private object MapObjectToDbEntity(JsonNode jsonNode, string contentType)
    {
      Type? contentTypeType = _allTypes.Find(type => type.Name == contentType);

      if (contentTypeType == null)
      {
        throw new KeyNotFoundException($"Could not find matching type for {contentTypeType}");
      }

      var content = Activator.CreateInstance(contentTypeType);

      var mapped = _mapper.Map(jsonNode, content!, jsonNode.GetType(), content!.GetType());

      return mapped;
    }

    public string FirstCharToUpperAsSpan(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }
      Span<char> destination = stackalloc char[1];
      input.AsSpan(0, 1).ToUpperInvariant(destination);
      return $"{destination}{input.AsSpan(1)}";
    }

  }
}
