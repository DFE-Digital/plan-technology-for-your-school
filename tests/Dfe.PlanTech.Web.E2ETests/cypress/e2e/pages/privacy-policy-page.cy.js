describe("Privacy Policy Page - Unauthenticated", () => {
  const url = "/";

  beforeEach(() => {
    cy.visit(url);
    cy.get(
      "footer.govuk-footer ul.govuk-footer__inline-list a.govuk-footer__link"
    )
      .contains("Privacy")
      .click();
    cy.url().should("contain", "/privacy");
    cy.injectAxe();
  });

  it("Should Have Heading", () => {
    cy.get("h1.govuk-heading-xl").should("exist");
  });

  it("Should Have Home Button", () => {
    cy.get('a:contains("Home")')
      .should("exist")
      .should("have.attr", "href")
      .and("include", "/");
  });

  it("Should Have Content", () => {
    cy.get("p").should("exist");
  });

  it("Passes Accessibility Testing", () => {
    cy.runAxe();
  });
});
