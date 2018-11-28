# Kudu to Slack relay webhook

Azure function that relays [Kudu](https://github.com/projectkudu/kudu/) deployment messages to Slack.
[![Build status](https://ci.appveyor.com/api/projects/status/sjo9glvvi4oj1k1j?svg=true)](https://ci.appveyor.com/project/VidarKongsli/kudu-to-slack-func)

## Install

Use one of the following:

### Option 1

Click here: [![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/?ptmpl=azuredeploy.parameters.json)
...and follow the instructions.

### Option 2

1. Make sure you have the [Azure PowerShell Module](https://docs.microsoft.com/en-us/powershell/azure/install-azurerm-ps) installed.
1. Download  [azuredeploy.json](./azuredeploy.json)
1. Log in to your Azure tenant with `Login-AzureRmAccount`.
1. Create a resource group `New-AzureRmResourceGroup -Name <name> -Location <location>`
1. Deploy with `New-AzureRmResourceGroupDeployment -Name <some name> -ResourceGroupName <rg name> -TemplateFile .\azuredeploy.json -TemplateParameterObject @{siteName='<site name>'}`

## Usage

* Make a webhook for posting to your Slack team at `https://<team-name>.slack.com/apps/manage/custom-integrations`
* Set up an access code for your function in the Azure portal
* Craft the URL to your function, using the format `https://<name>.azurewebsites.net/api/kudu2slackrelay?code=<generated code>&clientid=<clientid>&webhook=<slack-webhook-url>`
  * *name* : the name given to your function app
  * *generated code*: the access code generated for the function
  * *clientid*: name that corresponds to the generated code
  * *slack-webhook-url*: the webhook in Slack that the function will relay the call to, something like `https://hooks.slack.com/services/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`
* Enter the function url to trigger on the PostDeployment event in Kudu at `https://<name>.scm.azurewebsites.net/WebHooks`
