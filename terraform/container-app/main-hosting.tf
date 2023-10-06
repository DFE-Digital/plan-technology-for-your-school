module "main_hosting" {
  source = "github.com/DFE-Digital/terraform-azurerm-container-apps-hosting"

  ###########
  # General #
  ###########
  environment    = local.environment
  project_name   = local.project_name
  azure_location = local.azure_location
  tags           = local.tags

  #################
  # Container App #
  #################
  enable_container_registry = true
  image_name                = local.container_app_image_name
  container_port            = local.container_port
  container_secret_environment_variables = {
    "AZURE_CLIENT_ID" = azurerm_user_assigned_identity.user_assigned_identity.client_id,
    "KeyVaultName"    = local.kv_name
  }

  container_environment_variables = {
    "Kestrel__Endpoints__Http__Url"       = local.kestrel_endpoint,
    "ASPNETCORE_FORWARDEDHEADERS_ENABLED" = "true"
  }

  container_app_identities = {
    type         = "UserAssigned",
    identity_ids = [azurerm_user_assigned_identity.user_assigned_identity.id]
  }

  #############
  # Azure SQL #
  #############
  enable_mssql_database              = true
  mssql_database_name                = "${local.resource_prefix}-sqldb"
  mssql_server_admin_password        = random_password.az_sql_password.result
  mssql_server_public_access_enabled = true
  mssql_azuread_admin_username       = local.az_sql_azuread_admin_username
  mssql_azuread_admin_object_id      = local.az_sql_azuread_admin_objectid
  mssql_azuread_auth_only            = local.az_use_azure_ad_auth_only

  ##############
  # Networking #
  ##############
  container_apps_infra_subnet_service_endpoints = ["Microsoft.KeyVault"]
}

resource "random_password" "az_sql_password" {
  length           = 32
  special          = true
  override_special = "!#$%&*()-_=+[]{}<>:?"
}