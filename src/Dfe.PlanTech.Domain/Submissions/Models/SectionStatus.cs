﻿using Dfe.PlanTech.Domain.Questionnaire.Interfaces;

namespace Dfe.PlanTech.Domain.Submissions.Models;

public class SectionStatus
{
    public string SectionId { get; set; } = null!;

    public int Completed { get; set; }

    public string? Maturity { get; set; }

    public DateTime DateCreated { get; set; }
}

public record SectionStatusNew
{
    public string SectionId { get; set; } = null!;

    public bool Completed { get; set; }

    public string? Maturity { get; set; }

    public DateTime DateCreated { get; set; }
}