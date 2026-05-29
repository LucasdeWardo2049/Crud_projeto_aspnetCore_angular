$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$apiDir = Join-Path $root "EmployeeSchedule\EmployeeSchedule.Api"
$backendUrl = "http://localhost:5000"
$oldAspNetCoreEnvironment = $env:ASPNETCORE_ENVIRONMENT

function Test-Command($name) {
    return [bool](Get-Command $name -ErrorAction SilentlyContinue)
}

if (-not (Test-Command "dotnet")) {
    throw "dotnet nao encontrado no PATH. Instale o .NET SDK antes de rodar o backend."
}

$portInUse = Get-NetTCPConnection -LocalPort 5000 -State Listen -ErrorAction SilentlyContinue
if ($portInUse) {
    $processIds = ($portInUse | Select-Object -ExpandProperty OwningProcess -Unique) -join ", "
    throw "A porta 5000 ja esta em uso. Processo(s): $processIds"
}

Push-Location $apiDir
try {
    $env:ASPNETCORE_ENVIRONMENT = "Development"

    Write-Host "Restaurando backend..."
    dotnet restore

    Write-Host "Aplicando migrations..."
    dotnet ef database update

    Write-Host "Backend no ar em $backendUrl"
    Write-Host "Swagger: $backendUrl/swagger"
    dotnet run --urls $backendUrl
}
finally {
    $env:ASPNETCORE_ENVIRONMENT = $oldAspNetCoreEnvironment
    Pop-Location
}
