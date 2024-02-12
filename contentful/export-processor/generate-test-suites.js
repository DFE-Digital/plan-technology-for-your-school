import contentfulExport from "contentful-export";
import "dotenv/config";
import DataMapper from "#src/data-mapper";

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
