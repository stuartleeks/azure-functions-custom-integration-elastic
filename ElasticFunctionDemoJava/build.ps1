mvn clean package -B

$functionJsonContent = Get-Content .\target\azure-functions\ElasticFunctionDemoJava-1540824129043\HttpTrigger-Java\function.json -Raw
$functionJsonContent = $functionJsonContent.Replace(" } ]", "},`n  {`n    `"type`": `"elastic`",`n    `"name`": `"elasticMessage`",`n    `"direction`": `"out`",`n    `"index`": `"people`",`n    `"indexType`" : `"wibble`"`n  } ]")

Set-Content .\target\azure-functions\ElasticFunctionDemoJava-1540824129043\HttpTrigger-Java\function.json -Value $functionJsonContent
