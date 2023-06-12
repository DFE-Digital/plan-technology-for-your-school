resource "azurerm_user_assigned_identity" "managed_identity" {
  name                = local.user_identity_name
  location            = local.azure_location
  resource_group_name = local.resource_group_name
}