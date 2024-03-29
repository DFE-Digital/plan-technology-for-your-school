using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dfe.PlanTech.DataValidation.Tests;

public static class ContentfulHelpers
{
  public static string GetEntryId(this JsonNode jsonNode)
  {
    return jsonNode["id"]?.GetValue<string>() ?? throw new JsonException($"Couldn't find id for {jsonNode}");
  }

  public static JsonNode WithoutLocalisation(this JsonNode jsonNode)
  {
    var inner = jsonNode["en-US"] ?? throw new JsonException($"No localisation found for {jsonNode}");

    return inner.Deserialize<JsonNode>()!;
  }
}