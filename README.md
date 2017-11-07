# Kudu to Slack relay webhook
Azure function that relays [Kudu](https://github.com/projectkudu/kudu/) deployment messages to Slack.

## Install
### Fork this repo
### Provision infrastructure
The script below will set up an Azure function app with a consumption plan in Azure.
```powershell
tools\deploy.ps1 -subscriptionId <subscription> -resourceGroupName <rg> `
  -resourceGroupLocation <location> -deploymentName <some name>
```
#### Example
```powershell
tools\deploy.ps1 -subscriptionId 5eed131f-a31a-41ee-8159-b9610c7e7389 `
  -resourceGroupName slackr-rg -resourceGroupLocation westeurope `
  -deploymentName slackr
```

### Set up function deployment
This will deploy the function to your function app instance. It will also set up automatic deployment when you check in to the master branch of your repository.
```powershell
 az functionapp deployment source config -n <name> -g <rg> --repo-url <url-to-your-repo> --branch master
```
* Hostname will be `<name>.azurewebsites.net`
* Repository URL can be for either a public or a private repository.
#### Example
```powershell
 az functionapp deployment source config -n slackr -g slackr-rg `
  --repo-url git@github.com:vidarkongsli/kudu-to-slack-func.git `
  --branch master
```

## Usage
* Make a webhook for posting to your Slack team at `https://<team-name>.slack.com/apps/manage/custom-integrations`
* Set up an access code for your function in the Azure portal
* Craft the URL to your function, using the format `https://<name>.azurewebsites.net/api/kudu2slackrelay?code=<generated code>&clientid=<clientid>&webhook=<slack-webhook-url>`
  * *name* : the name given to your function app
  * *generated code*: the access code generated for the function
  * *clientid*: name that corresponds to the generated code
  * *slack-webhook-url*: the webhook in Slack that the function will relay the call to, something like `https://hooks.slack.com/services/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`
* Enter the function url to trigger on the PostDeployment event in Kudu at `https://<name>.scm.azurewebsites.net/WebHooks`

