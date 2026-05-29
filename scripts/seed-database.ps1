$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$apiSettingsPath = Join-Path $root "EmployeeSchedule\EmployeeSchedule.Api\appsettings.json"
$apiDevelopmentSettingsPath = Join-Path $root "EmployeeSchedule\EmployeeSchedule.Api\appsettings.Development.json"
$seedSqlPath = Join-Path $PSScriptRoot "seed-database.sql"

function Convert-ConnectionStringToHash($connectionString) {
    $settings = @{}

    foreach ($part in ($connectionString -split ";")) {
        if ($part -match "^\s*([^=]+)=(.*)$") {
            $settings[$matches[1].Trim()] = $matches[2].Trim()
        }
    }

    return $settings
}

function Resolve-PsqlPath {
    $command = Get-Command "psql" -ErrorAction SilentlyContinue
    if ($command) {
        return $command.Source
    }

    $basePaths = @(
        $env:ProgramFiles,
        ${env:ProgramFiles(x86)}
    ) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }

    $versions = @("18", "17", "16", "15", "14", "13")
    foreach ($basePath in $basePaths) {
        foreach ($version in $versions) {
            $candidate = Join-Path $basePath "PostgreSQL\$version\bin\psql.exe"
            if (Test-Path $candidate) {
                return $candidate
            }
        }
    }

    throw "psql nao encontrado. Rode o conteudo de scripts\seed-database.sql no pgAdmin ou instale o PostgreSQL client."
}

if (-not (Test-Path $apiSettingsPath)) {
    throw "appsettings.json nao encontrado em $apiSettingsPath"
}

if (-not (Test-Path $seedSqlPath)) {
    throw "Arquivo seed-database.sql nao encontrado em $seedSqlPath"
}

$config = Get-Content $apiSettingsPath -Raw | ConvertFrom-Json
$connectionString = $config.ConnectionStrings.DefaultConnection

if (Test-Path $apiDevelopmentSettingsPath) {
    $developmentConfig = Get-Content $apiDevelopmentSettingsPath -Raw | ConvertFrom-Json
    $developmentConnectionString = $developmentConfig.ConnectionStrings.DefaultConnection

    if (-not [string]::IsNullOrWhiteSpace($developmentConnectionString)) {
        $connectionString = $developmentConnectionString
    }
}

$settings = Convert-ConnectionStringToHash $connectionString
$psqlPath = Resolve-PsqlPath

$requiredKeys = @("Host", "Port", "Database", "Username", "Password")
foreach ($key in $requiredKeys) {
    if (-not $settings.ContainsKey($key) -or [string]::IsNullOrWhiteSpace($settings[$key])) {
        throw "Connection string sem o valor obrigatorio: $key"
    }
}

$oldPassword = $env:PGPASSWORD
$env:PGPASSWORD = $settings["Password"]
$dbHost = $settings["Host"]
$dbPort = $settings["Port"]
$dbUser = $settings["Username"]
$dbName = $settings["Database"]

try {
    Write-Host "Aplicando seed no banco $dbName..."
    & $psqlPath `
        -h $dbHost `
        -p $dbPort `
        -U $dbUser `
        -d $dbName `
        -v ON_ERROR_STOP=1 `
        -f $seedSqlPath

    if ($LASTEXITCODE -ne 0) {
        throw "Seed falhou com codigo $LASTEXITCODE"
    }

    Write-Host "Seed aplicado com sucesso."
}
finally {
    $env:PGPASSWORD = $oldPassword
}
