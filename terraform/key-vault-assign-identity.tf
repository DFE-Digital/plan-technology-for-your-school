resource "null_resource" "keyvault-assign-identity" {
  depends_on = [module.main_hosting]
  triggers = {
    #always_run = "${timestamp()}"
    managed_identity_id = azurerm_user_assigned_identity.managed_identity.id
  }

  provisioner "local-exec" {
    interpreter = ["/bin/bash", "-c"]
    command     = local.keyvault-assign-identity
  }
}