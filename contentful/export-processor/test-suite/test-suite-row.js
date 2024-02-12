export default class TestSuiteRow {
  testReference;
  appendixRef;
  adoTag;
  subTopic;
  testScenario;
  preConditions;
  testSteps;
  expectedOutcome;
  testApproved;

  constructor({
    testReference,
    appendixRef,
    adoTag,
    subTopic,
    testScenario,
    preConditions,
    testSteps,
    expectedOutcome,
    testApproved,
  }) {
    this.testReference = testReference;
    this.appendixRef = appendixRef;
    this.adoTag = adoTag;
    this.subTopic = subTopic;
    this.testScenario = testScenario;
    this.preConditions = preConditions;
    this.testSteps = testSteps;
    this.expectedOutcome = expectedOutcome;
    this.testApproved = testApproved;
  }
}
