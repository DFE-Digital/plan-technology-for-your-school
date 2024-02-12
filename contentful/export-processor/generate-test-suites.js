import contentfulExport from "contentful-export";
import "dotenv/config";
import DataMapper from "#src/data-mapper";
import TestSuite from "#src/test-suite/test-suite";

const options = {
  spaceId: process.env.SPACE_ID,
  deliveryToken: process.env.DELIVERY_TOKEN,
  managementToken: process.env.MANAGEMENT_TOKEN,
  environmentId: "dev",
  host: "api.contentful.com",
  skipEditorInterfaces: true,
  skipRoles: true,
  skipWebhooks: true,
};

const exportedData = await contentfulExport(options);

const mapped = new DataMapper(exportedData);

let index = 1;

const testSuites = Object.values(mapped.mappedSections).map((section) => {
  const testSuite = new TestSuite({
    subtopic: section,
    testReferenceIndex: index,
  });

  console.log(testSuite);

  index = testSuite.testReferenceIndex;

  return testSuite;
});

const asArray = Array.from(testSuites);

console.log(asArray);
