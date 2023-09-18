using Dfe.PlanTech.Application.Responses.Commands;
using Dfe.PlanTech.Application.Responses.Interface;
using Dfe.PlanTech.Domain.Content.Models;
using Dfe.PlanTech.Domain.Questionnaire.Models;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Dfe.PlanTech.Application.UnitTests.Responses.Commands;

public class ProcessCheckAnswerDtoCommandTests
{
  [Fact]
  public async Task Should_Remove_Detached_Questions()
  {
    var questionIds = new[] { "QuestionRef1", "QuestionRef2", "QuestionRef3" };
    var answerIds = new[] { "AnswerRef1", "AnswerRef2", "AnswerRef3" };

    CheckAnswerDto response = new()
    {
      SubmissionId = 1,
      Responses = new List<QuestionWithAnswer>(){
        new(){
          QuestionRef = questionIds[0],
          AnswerRef = answerIds[0],
          AnswerText = "Answer 1 text",
          QuestionText = "Question 1 text",
          DateCreated = DateTime.Now,
        },
        new(){
          QuestionRef = questionIds[1],
          AnswerRef = answerIds[1],
          AnswerText = "Answer 2 text",
          QuestionText = "Question 2 text",
          DateCreated = DateTime.Now
        },
        new(){
          QuestionRef = questionIds[2],
          AnswerRef = answerIds[2],
          AnswerText = "Answer 3 text",
          QuestionText = "Question 3 text",
          DateCreated = DateTime.Now
        },
      }
    };

    var questions = new Question[] {new()
    {
      Sys = new SystemDetails()
      {
        Id = questionIds[0]
      },
      Answers = new Answer[] {
          new(){
            Sys = new SystemDetails(){
              Id = answerIds[0]
            },
          }
        }
    },new(){
      Sys = new SystemDetails()
      {
        Id = questionIds[1],
      },
        Answers = new Answer[] {
          new() {
            Sys = new SystemDetails() {
              Id = answerIds[1]
            }
          }
        }
      },
    new()
    {
    Sys = new SystemDetails()
    {
      Id = questionIds[2],
    },
      Answers = new Answer[] {
        new() {
          Sys = new SystemDetails() {
            Id = answerIds[2]
          }
        }
      }
    }
  };

    questions[0].Answers[0] = new Answer()
    {
      Sys = new SystemDetails()
      {
        Id = answerIds[0]
      },
      NextQuestion = questions[2]
    };

    var section = new Section()
    {
      Name = "Test section",
      Sys = new SystemDetails() { Id = "ABCD" },
      Questions = questions,
    };

    var getLatestResponsesQuery = Substitute.For<IGetLatestResponsesQuery>();
    getLatestResponsesQuery.GetLatestResponses(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                         .Returns((callinfo) => response);
                         
    var processCheckAnswerDtoCommand = new ProcessCheckAnswerDtoCommand(getLatestResponsesQuery);


    var checkAnswerDto = await processCheckAnswerDtoCommand.GetCheckAnswerDtoForSection(3, section);

    Assert.NotNull(checkAnswerDto);

    Assert.Equal(checkAnswerDto.SubmissionId, response.SubmissionId);
    Assert.Equal(2, checkAnswerDto.Responses.Count);
  }

  [Fact]
  public async Task Should_Return_Null_When_No_Responses()
  {
    CheckAnswerDto? response = null;

    var getLatestResponsesQuery = Substitute.For<IGetLatestResponsesQuery>();
    getLatestResponsesQuery.GetLatestResponses(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                        .Returns(Task.FromResult(response));

    var processCheckAnswerDtoCommand = new ProcessCheckAnswerDtoCommand(getLatestResponsesQuery);

    var section = new Section() { Sys = new SystemDetails() { Id = "ABCD" } };

    var checkAnswerDto = await processCheckAnswerDtoCommand.GetCheckAnswerDtoForSection(3, section);

    Assert.Null(checkAnswerDto);
  }
}