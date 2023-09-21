locals {
  ###########
  # General #
  ###########
  current_user_id     = coalesce(var.msi_id, data.azurerm_client_config.current.object_id)
  project_name        = var.project_name
  environment         = var.environment
  azure_location      = var.azure_location
  resource_prefix     = "${local.environment}${local.project_name}"
  resource_group_name = "${local.environment}${local.project_name}"
  tags = {
    "Environment"      = var.az_tag_environment,
    "Service Offering" = var.az_tag_product,
    "Product"          = var.az_tag_product
  }

  #################
  # Container App #
  #################
  container_app_image_name = "plan-tech-app"
  container_app_name       = "${local.resource_prefix}-${local.container_app_image_name}"
  kestrel_endpoint         = var.az_app_kestrel_endpoint

  ##############
  # Front Door #
  ##############
  cdn_frontdoor_origin_host_header_override = var.cdn_frontdoor_origin_host_header_override

  ####################
  # Managed Identity #
  ####################
  user_identity_name = "${local.resource_prefix}-mi"

  ##############
  # Networking #
  ##############
  vnet_name   = "${local.resource_prefix}default"
  subnet_name = "${local.resource_prefix}containerappsinfra"

  #############
  # Azure SQL #
  #############
  az_sql_connection_string      = "Server=tcp:${local.resource_prefix}.database.windows.net,1433;Initial Catalog=${local.resource_prefix}-sqldb;Authentication=Active Directory Default; Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  az_sql_admin_userid           = "${local.resource_prefix}-${var.az_sql_admin_userid_postfix}"
  az_sql_azuread_admin_username = var.az_sql_azuread_admin_username
  az_sql_azuread_admin_objectid = var.az_sql_azuread_admin_objectid
  az_use_azure_ad_auth_only     = var.az_tag_environment != "Dev"

  ##################
  # Azure KeyVault #
  ##################
  kv_name = "${local.environment}${local.project_name}-kv"

  ###########
  # Scripts #
  ###########
  container_app-assign-identity         = "timeout 15m ${path.module}/scripts/assign-user-identity-to-app.sh -n \"${local.container_app_name}\" -g \"${local.resource_group_name}\" -u \"${local.user_identity_name}\""
  keyvault-add-vnet-restriction_command = "timeout 15m ${path.module}/scripts/add-keyvault-service-endpoint-to-app.sh -c \"${local.container_app_name}\" -g \"${local.resource_group_name}\" -v \"${local.vnet_name}\" -n \"${local.subnet_name}\" -k \"${local.kv_name}\""
}