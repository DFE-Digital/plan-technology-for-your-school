using System.Text.Json.Serialization;

namespace Dfe.PlanTech.Domain.Establishments.Models;

public class EstablishmentDto
{
    public const string InvalidEstablishmentErrorMessage = $"Both {nameof(Urn)} and {nameof(Ukprn)} are invalid";

    [JsonPropertyName("ukprn")]
    public string? Ukprn { get; set; }

    [JsonPropertyName("urn")] public string? Urn { get; set; }

    [JsonPropertyName("type")]
    public EstablishmentTypeDto Type { get; set; } = new EstablishmentTypeDto();

    [JsonPropertyName("name")]
    public string OrgName { get; set; } = null!;

    public bool IsValid => References().Any(reference => !string.IsNullOrEmpty(reference));

    public string Reference => References().FirstOrDefault(reference => !string.IsNullOrEmpty(reference)) ??
                                throw new InvalidOperationException(InvalidEstablishmentErrorMessage);

    private IEnumerable<string?> References()
    {
        yield return Urn;
        yield return Ukprn;
    }
}