# local.settings.json has "AzureWebJobs_ExtensionsPath" : "..\\Elastic.FunctionBinding\\bin\\Debug\\"
# function BuildExtensions {
#     dotnet build ..\Elastic.FunctionBinding\
#     if (Test-Path "bin"){
#         Remove-Item "bin" -Recurse
#     }
#     Copy-Item "..\\Elastic.FunctionBinding\\bin\\Debug\\netstandard2.0\\" "bin" -Recurse
#     Set-Content -Value "*" -Path "bin\.gitignore"
# }
# BuildExtensions

dotnet build ..\Elastic.FunctionBinding\
func extensions install
func start
