# Kudu to Slack relay webhook
Azure function that relays [https://github.com/projectkudu/kudu/](Kudu) deployment messages to Slack.

## Install
### Fork this repo
### Provision infrastructure in Azure
```powershell
tools\deploy.ps1 -subscriptionId <subscription> -resourceGroupName <rg> -resourceGroupLocation <location> -deploymentName <some name>
```
### Set up function deployment
```powershell
 az functionapp deployment source config -n <name> -g <rg> --repo-url <url-to-your-repo> --branch master
```
* Hostname will be `<name>.azurewebsites.net`
* Repository URL can be for either a public or a private repository.

## Usage
* Make a webhook for posting to your Slack team at `https://<team-name>.slack.com/apps/manage/custom-integrations`
* Set up an access code for your function in the Azure portal
* Craft the URL to your function, using the format `https://<name>.azurewebsites.net/api/kudu2slackrelay?code=<generated code>&clientid=<clientid>&webhook=<slack-webhook-url>`
  * *name* : the name given to your functiona app
  * *generated code*: the access code generated for the function
  * *clientid*: name that corresponds to the generated code
  * *slack-webhook-url*: the webhook in Slack that the function will relay the call to, something like `https://hooks.slack.com/services/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`
* Enter the function url to trigger on the PostDeployment event in Kudu at `https://<name>.scm.azurewebsites.net/WebHooks`

