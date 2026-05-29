$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$webDir = Join-Path $root "EmployeeSchedule\employee-schedule-web"
$frontendUrl = "http://localhost:4200"

function Test-Command($name) {
    return [bool](Get-Command $name -ErrorAction SilentlyContinue)
}

if (-not (Test-Command "npm")) {
    throw "npm nao encontrado no PATH. Instale o Node.js antes de rodar o frontend."
}

$portInUse = Get-NetTCPConnection -LocalPort 4200 -State Listen -ErrorAction SilentlyContinue
if ($portInUse) {
    $processIds = ($portInUse | Select-Object -ExpandProperty OwningProcess -Unique) -join ", "
    throw "A porta 4200 ja esta em uso. Processo(s): $processIds"
}

Push-Location $webDir
try {
    if (-not (Test-Path "node_modules")) {
        Write-Host "Instalando dependencias do frontend..."
        npm install
    }

    Write-Host "Frontend no ar em $frontendUrl"
    npx ng serve --host localhost --port 4200
}
finally {
    Pop-Location
}
