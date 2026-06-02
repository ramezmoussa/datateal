using './main.bicep'

param location = 'swedencentral'
param clusterName = 'aks-datateal-dev'
param acrName = 'acrdatatealdev' // Must be globally unique; change if the name is already taken
param psqlName = 'psql-aks-datateal-dev'
param storageAccountName = 'stdatatealdev' // Must be globally unique; change if the name is already taken
param nodeResourceGroupName = 'mrg-datateal-dev'
param systemNodePoolVmSize = 'Standard_D2as_v5'
param firewallWhitelistIp = ''
param postgresAdminPassword = ''

// Set this to the object ID of the managed identity or service principal that
// the Control Plane API runs as, so it can create and delete node pools.
// You can retrieve it after deploying the API and re-run the deployment.
param apiPrincipalId = ''
