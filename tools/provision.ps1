param(
    [Parameter(Mandatory)]
    [ValidateLength(5,20)]
    $name,
    [Parameter(Mandatory=$false)]
    $location = 'westeurope',
    [Parameter(Mandatory=$false)]
    $storage_sku = 'Standard_GRS',
    [Parameter(Mandatory=$false)]
    $resourceGroup = $null
)
if (-not($resourceGroup)) {
    $resourceGroup = $name
}
if (-not((az group exists -n $resourceGroup) -eq 'true')) {
    az group create -n $resourceGroup -l $location    
}
$accountName = "${name}01"
if(-not(az storage account show -n $accountName -g $resourceGroup)) {
    az storage account create -n $accountName `
        -g $resourceGroup --sku $storage_sku `
        -l $location
}
az functionapp create -s $accountName `
    -c $location -n $name -g $resourceGroup 
