describe("landing page", () => {
  const url = Cypress.env("URL") + "/self-assessment";

  beforeEach(() => {
    cy.loginWithEnv(url);
    cy.origin(Cypress.env("URL"), () => {
      cy.get("ul.app-task-list__items > li a").first().click();
      cy.get('a.govuk-button').contains('Continue').click();
    });
  });

  it("should contain form", () => {
    cy.origin(Cypress.env("URL"), () => {
      cy.get("form").should("exist");
    });
  });

  it("should contain heading", () => {
    cy.origin(Cypress.env("URL"), () => {
      cy.get("form h1.govuk-fieldset__heading").should("exist");

      cy.get("form h1.govuk-fieldset__heading")
        .invoke("text")
        .should("have.length.greaterThan", 1);
    });
  });

  it("should contain answers", () => {
    cy.origin(Cypress.env("URL"), () => {
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
  });

  it("should have submit button", () => {
    cy.origin(Cypress.env("URL"), () => {
      cy.get("form button.govuk-button").should("exist");
    });
  });

  it("should navigate to next page on submit", () => {
    cy.origin(Cypress.env("URL"), () => {

      const path = cy.location("pathname");

      cy.get("form div.govuk-radios div.govuk-radios__item input")
        .first()
        .click();

      cy.get("form button.govuk-button")
        .contains("Save and continue")
        .click();

      cy.location("pathname", { timeout: 60000 })
        .should("not.include", path)
        .and("include", "/question");
    });
  });

  it("should have back button", () => {
    cy.origin(Cypress.env("URL"), () => {
      cy.get("a.govuk-back-link")
        .should("exist")
        .invoke("text")
        .should("equal", "Back");
    });
  });

  it("should have back button that navigates to last question once submitted", () => {
    cy.origin(Cypress.env("URL"), { args: { FIRST_QUESTION } }, ({ FIRST_QUESTION }) => {
      cy.get("form div.govuk-radios div.govuk-radios__item input")
        .first()
        .click();

      cy.get("form button.govuk-button").click();

      cy.location("pathname", { timeout: 60000 })
        .should("not.include", FIRST_QUESTION)
        .and("include", "/question");

      cy.get("a.govuk-back-link")
        .should("exist")
        .and("have.attr", "href")
        .and("include", FIRST_QUESTION);
    });
  });

});
