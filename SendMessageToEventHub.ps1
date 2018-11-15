param(
    # The message contents to send
    [Parameter(Mandatory = $true)]
    [string]
    $message
)

function ConvertConnectionStringToDictionary {
    param (
        [string]
        $connectionString
    )

    $result = @{}
    $connectionString.Split(";") | ForEach-Object { 
        $index = $PSItem.IndexOf("=")
        $key = $PSItem.Substring(0, $index)
        $value = $PSItem.Substring($index + 1)
        $result[$key] = $value
    }
    return [PSCustomObject]$result
}



function GenerateSasToken() {
    param (
        [string]
        $URI,
        [string]
        $AccessPolicyName,
        [string]
        $AccessPolicyKey
    )
    # Based on https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token#powershell
    [Reflection.Assembly]::LoadWithPartialName("System.Web")| out-null
    
    $encodedUri = [System.Web.HttpUtility]::UrlEncode($URI)
    #Token expires now+300
    $expires = ([DateTimeOffset]::Now.ToUnixTimeSeconds()) + 3000
    $signatureString = "$encodedUri`n" + [string]$expires
    $hmac = New-Object System.Security.Cryptography.HMACSHA256
    $hmac.key = [Text.Encoding]::ASCII.GetBytes($AccessPolicyKey)
    $signature = $hmac.ComputeHash([Text.Encoding]::ASCII.GetBytes($signatureString))
    $signature = [Convert]::ToBase64String($signature)
    $signature = [System.Web.HttpUtility]::UrlEncode($signature)
    $sasToken = "SharedAccessSignature sr=$encodedUri&sig=$signature&se=$Expires&skn=$AccessPolicyName"
    $sasToken
}


function SendMessageToEventHub($message) {
    $settings = Get-Content "ElasticFunctionDemoCSharp/local.settings.json" | ConvertFrom-Json
    $eventHubConnectionString = $settings.Values.EventHubConnection
    $eventHubConnectionInfo = ConvertConnectionStringToDictionary( $eventHubConnectionString)
    $eventHubName = $eventHubConnectionInfo.EntityPath

    $eventHubRootUri = $eventHubConnectionInfo.Endpoint.Replace("sb://", "").TrimEnd("/")
    $sasToken = GenerateSasToken "$eventHubRootUri/$eventHubName" $eventHubConnectionInfo.SharedAccessKeyName $eventHubConnectionInfo.SharedAccessKey

    $headers = @{
        "Authorization" = $sasToken;
        "Content-Type"  = "application/atom+xml;type=entry;charset=utf-8";
    }

    Invoke-RestMethod -Uri "https://$eventHubRootUri/$eventHubName/messages" -Method "POST" -Headers $headers -Body $message
}

SendMessageToEventHub $message
Write-Host "Sent: $message"