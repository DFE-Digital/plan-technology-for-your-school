describe("landing page", () => {
  const FIRST_QUESTION = "/question/6Z9tFZQmLhiMNDe2IA5qYE";

  const url = Cypress.env("URL") + FIRST_QUESTION;

  beforeEach(() => {
    cy.visit(url);
  });

  it("should contain form", () => {
    cy.get("form").should("exist");
  });

  it("should contain heading", () => {
    cy.get("form h1.govuk-fieldset__heading").should("exist");

    cy.get("form h1.govuk-fieldset__heading")
      .invoke("text")
      .should("have.length.greaterThan", 1);
  });

  it("should contain answers", () => {
    cy.get("form div.govuk-radios div.govuk-radios__item")
      .should("exist")
      .and("have.length.greaterThan", 1)
      .each((item) => {
        cy.wrap(item)
          .get("label")
          .should("exist")
          .invoke("text")
          .should("have.length.greaterThan", 1);
      });
  });

  it("should have submit button", () => {
    cy.get("form button.govuk-button").should("exist");
  });

  it("should navigate to next page on submit", () => {
    cy.get("form div.govuk-radios div.govuk-radios__item input")
      .first()
      .click();

    cy.get("form button.govuk-button").click();

    cy.location("pathname", { timeout: 60000 })
      .should("not.include", FIRST_QUESTION)
      .and("include", "/question");
  });
});
