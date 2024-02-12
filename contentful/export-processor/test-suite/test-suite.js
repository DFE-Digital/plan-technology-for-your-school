import TestSuiteRow from "#src/test-suite/test-suite-row";
//import Appendix from "#/src/test-suite/appendix";

const ADO_TAG = "Functional";

export default class TestSuiteForSubTopic {
  canNavigateToSubtopic;
  canSaveAnswersForQuestions;
  saveAndContinueWithoutSelectingAnswer;
  savesPartiallyCompletedSubtopic;
  canContinuePartiallyCompletedSubtopic;
  differentUserCanUpdateAndResubmitResponses;
  manuallyNavigatesToCheckAnswerPageBeforeCompletion;
  manuallyNavigatesToQuestionAheadOnPath;
  manuallyNavigatesToQuestionAlreadyAnswered;
  manuallyNavigatesToQuestionNotOnPath;
  manuallyNavigatesToRecommendationPageBeforeCompletingSubtopic;
  manuallyNavigatesToRecommendationPageDifferentFromAssignedRecommendation;
  userAttemptsToLoginOnSpecificQuestionAfterTimingOut;
  unableToRetrieveQuestionListOnCheckAnswerPage;
  unableToRetrieveSelectedAnswerOnCheckAnswerPage;
  unableToCalculateRecommendationOnCheckAnswerPage;
  unableToRetrieveSuccessRecomendationMessageOnSelfAssessmentPage; //Is this a possible thing?
  userReceivesLowScoringLogic;
  userReceivesMediumScoringLogic;
  userReceivesHighScoringLogic;
  userCanChangeAnswersOnCheckAnswersPage;
  userCanLogOut;

  //appendix = new Appendix();

  subtopic;
  testReferenceIndex;

  constructor({ subtopic, testReferenceIndex }) {
    this.subtopic = subtopic;
    this.testReferenceIndex = testReferenceIndex;

    this.generateCanNavigateToSubtopic();
  }
  /*
    testReference,
    appendixRef,
    adoTag,
    subTopic,
    testScenario,
    preConditions,
    testSteps,
    expectedOutcome,
    testApproved,
*/
  generateCanNavigateToSubtopic() {
    const row = new TestSuiteRow({
      testReference: this.generateTestReference(),
      adoTag: ADO_TAG,
      subtopic: this.subtopic.name,
      testScenario: `User can naviagte to the ${this.subtopic.name} subtopic from the self-assessment page`,
      preConditions: "User is signed into the DfE Sign in service",
      testSteps: `1 - Navigate to ${this.subtopic.name} subtopic`,
      expectedOutcome: `1 - User sees ${this.subtopic.name} interstitial page`,
    });

    this.canNavigateToSubtopic = row;
  }

  generateTestReference() {
    return `Access_${this.testReferenceIndex++}`;
  }
}

function generateCanNavigateToSubtopic() {}
/*
User manually goes to the recommendation page via URL before completing the assessment questions for a subtopic
User manually goes to a different recommendation page via URL from the one their organisation has been assigned*/
