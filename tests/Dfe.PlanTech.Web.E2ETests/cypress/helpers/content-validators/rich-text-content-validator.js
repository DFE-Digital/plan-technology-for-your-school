import { parse } from "node-html-parser";
import TableValidator from "./rich-text-validators/table-validator";

const tableValidator = new TableValidator();

function ValidateRichTextContent(parent) {
  if (!parent?.content) return;

  for (const child of parent.content) {
    validateByNodeType(child);
  }
}

export default ValidateRichTextContent;
const regex = /(\r\n|\n|\r)/g;

function validateByNodeType(content) {
  switch (content.nodeType) {
    case "paragraph": {
      validateParagraph(content);
      break;
    }

    case "table": {
      tableValidator.validateTable(content);
      break;
    }

    case "unordered-list": {
      break;
    }

    case "ordered-list": {
      break;
    }
    default: {
      console.log(`not parsed nodetype ${content.nodeType}`);
    }
  }
}

function validateParagraph(content) {
  const expectedHtml = buildExpectedHtml(content).trim();

  const parsedElement = parse(expectedHtml);

  cy.get("p").then(($paragraphs) => {
    const paragraphHtmls = Array.from(
      Array.from($paragraphs.map((i, el) => Cypress.$(el).html()))
        .map((paragraph) => {
          const withoutWhitespaceEscaped = replaceWhiteSpace(paragraph).trim();

          return {
            original: withoutWhitespaceEscaped,
            parsed: parse(withoutWhitespaceEscaped),
          };
        })
        .filter((paragraph) => paragraph.original != "")
    );

    const anyMatches = paragraphHtmls.find(
      (paragraph) =>
        paragraph.original == expectedHtml ||
        paragraph.original.indexOf(expectedHtml) != -1 ||
        paragraph.parsed.innerHTML?.indexOf(parsedElement.innerHTML) != -1 ||
        paragraph.parsed.innerText
          ?.trim()
          .indexOf(parsedElement.innerText.trim()) != -1
    );

    if (!anyMatches) {
      console.error(`Could not find match for content`, expectedHtml, content);
    }

    expect(anyMatches).to.exist;
  });
}

function buildExpectedHtml(content) {
  let html = "";
  for (const child of content.content) {
    if (child.value) {
      html += child.value.replace(/\r\n/g, "\n");
    }

    if (child.nodeType == "hyperlink") {
      html += `<a href="${child.data.uri}" class="govuk-link">`;
    }

    if (child.content) {
      for (const grandchild of child.content) {
        if (grandchild.value) {
          html += grandchild.value;
        }
      }
    }

    if (child.nodeType == "hyperlink") {
      html += `</a>`;
    }
  }

  return replaceWhiteSpace(html).trim();
}

function replaceWhiteSpace(string) {
  return string
    .replace(/\s/g, " ")
    .replace(/&nbsp;/g, " ")
    .replace(/\s/g, " ")
    .replace(" ", " ")
    .replace(" ", " ")
    .replace(" ", " ");
}
