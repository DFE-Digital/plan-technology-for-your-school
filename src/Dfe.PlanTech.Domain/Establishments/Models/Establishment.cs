﻿using System.ComponentModel.DataAnnotations;

namespace Dfe.PlanTech.Domain.Establishments.Models;

public class Establishment
{
    public const int EstablishmentRefLength = 50;
    public const int EstablishmentTypeLength = 50;
    public const int OrgNameLength = 200;

    private string _establishmentRef = null!;
    private string _establishmentType = null!;
    private string _orgName = null!;

    public int Id { get; set; }

    [StringLength(EstablishmentRefLength)]
    public string EstablishmentRef
    {
        get => _establishmentRef;
        set
        {
            _establishmentRef = value.Length < EstablishmentRefLength ?
                                    value :
                                    value.AsSpan(0, EstablishmentRefLength - 1).ToString();
        }
    }

    [StringLength(EstablishmentTypeLength)]
    public string EstablishmentType
    {
        get => _establishmentType;
        set
        {
            _establishmentType = value.Length < EstablishmentTypeLength ?
                                    value :
                                    value.AsSpan(0, EstablishmentTypeLength - 1).ToString();
        }
    }

    [StringLength(OrgNameLength)]
    public string OrgName
    {
        get => _orgName;
        set
        {
            _orgName = value.Length < OrgNameLength ?
                                    value :
                                    value.AsSpan(0, OrgNameLength - 1).ToString();
        }
    }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public DateTime? DateLastUpdated { get; set; }
}

