namespace Dfe.PlanTech.Domain.Questionnaire.Interfaces;

public interface IQuestion
{
  public string Text { get; }

  public string? HelpText { get; }

  public string Param { get; }

  public string Slug { get; }
}

public interface IQuestion<TAnswer> : IQuestion
where TAnswer : IAnswer
{
  public TAnswer[] Answers { get; }
}