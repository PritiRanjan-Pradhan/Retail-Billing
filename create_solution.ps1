# PowerShell script to create RetailPOS solution and projects (Windows)
# Run from repository root

# Prerequisites check
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
	Write-Error "dotnet CLI not found in PATH. Install .NET 8 SDK first: https://dotnet.microsoft.com/en-us/download/dotnet/8.0"
	exit 1
}

if (-not (Test-Path -Path . -PathType Container)) {
	Write-Error "Run this script from the folder where you want the solution created."
	exit 1
}

# Create solution if it does not exist
$sln = "RetailPOS.sln"
if (-not (Test-Path $sln)) {
	Write-Host "Creating solution $sln"
	dotnet new sln -n RetailPOS
} else {
	Write-Host "Solution $sln already exists; skipping creation."
}

# Helper to create a project if missing
function Ensure-Project($template, $name, $framework) {
	$projDir = "$name"
	$csproj = Join-Path $projDir "$name.csproj"
	if (-not (Test-Path $csproj)) {
		Write-Host "Creating project $name"
		dotnet new $template -n $name -f $framework | Out-Null
	} else {
		Write-Host "Project $name already exists; skipping."
	}
}

# Create projects idempotently
Ensure-Project classlib RetailPOS.Domain net8.0
Ensure-Project classlib RetailPOS.Application net8.0
Ensure-Project classlib RetailPOS.Persistence net8.0
Ensure-Project classlib RetailPOS.Infrastructure net8.0
Ensure-Project wpf RetailPOS.WPF net8.0
Ensure-Project xunit RetailPOS.Application.Tests net8.0
Ensure-Project xunit RetailPOS.Infrastructure.Tests net8.0

# Create folder structure and move projects into src/tests if not already
if (-not (Test-Path src)) { New-Item -ItemType Directory -Force -Path src | Out-Null }
if (-not (Test-Path tests)) { New-Item -ItemType Directory -Force -Path tests | Out-Null }

Get-ChildItem -Path . -Directory | ForEach-Object {
	$name = $_.Name
	if ($name -like 'RetailPOS.*' -and $name -ne 'RetailPOS.WPF') {
		$target = Join-Path 'src' $name
		if (-not (Test-Path $target)) { Move-Item -Path $name -Destination src }
	}
}

# Move WPF explicitly
if ((Test-Path 'RetailPOS.WPF') -and -not (Test-Path 'src/RetailPOS.WPF')) { Move-Item -Path RetailPOS.WPF -Destination src }
# Move test projects
if ((Test-Path 'RetailPOS.Application.Tests') -and -not (Test-Path 'tests/RetailPOS.Application.Tests')) { Move-Item -Path RetailPOS.Application.Tests -Destination tests }
if ((Test-Path 'RetailPOS.Infrastructure.Tests') -and -not (Test-Path 'tests/RetailPOS.Infrastructure.Tests')) { Move-Item -Path RetailPOS.Infrastructure.Tests -Destination tests }

Write-Host "Adding projects to solution (idempotent)."
$projectsToAdd = @(
	'src/RetailPOS.Domain/RetailPOS.Domain.csproj',
	'src/RetailPOS.Application/RetailPOS.Application.csproj',
	'src/RetailPOS.Persistence/RetailPOS.Persistence.csproj',
	'src/RetailPOS.Infrastructure/RetailPOS.Infrastructure.csproj',
	'src/RetailPOS.WPF/RetailPOS.WPF.csproj',
	'tests/RetailPOS.Application.Tests/RetailPOS.Application.Tests.csproj',
	'tests/RetailPOS.Infrastructure.Tests/RetailPOS.Infrastructure.Tests.csproj'
)

foreach ($proj in $projectsToAdd) {
	if (Test-Path $proj) {
		$exists = dotnet sln RetailPOS.sln list | Select-String -Pattern ([regex]::Escape($proj))
		if (-not $exists) { dotnet sln RetailPOS.sln add $proj | Out-Null; Write-Host "Added $proj to solution." } else { Write-Host "$proj already in solution." }
	} else {
		Write-Host "Project file $proj not found; skipping add."
	}
}

Write-Host "Adding project references (idempotent)."
function Ensure-Reference($project, $reference) {
	if (-not (Test-Path $project)) { Write-Host "Project $project not found; skipping reference."; return }
	$refs = dotnet list $project reference
	if ($refs -notmatch [regex]::Escape($reference)) {
		dotnet add $project reference $reference | Out-Null
		Write-Host "Added reference $reference -> $project"
	} else {
		Write-Host "Reference $reference already exists in $project"
	}
}

# Application depends on Domain
Ensure-Reference 'src/RetailPOS.Application/RetailPOS.Application.csproj' 'src/RetailPOS.Domain/RetailPOS.Domain.csproj'
# Persistence depends on Application and Domain
Ensure-Reference 'src/RetailPOS.Persistence/RetailPOS.Persistence.csproj' 'src/RetailPOS.Application/RetailPOS.Application.csproj'
Ensure-Reference 'src/RetailPOS.Persistence/RetailPOS.Persistence.csproj' 'src/RetailPOS.Domain/RetailPOS.Domain.csproj'
# Infrastructure depends on Application and Persistence
Ensure-Reference 'src/RetailPOS.Infrastructure/RetailPOS.Infrastructure.csproj' 'src/RetailPOS.Application/RetailPOS.Application.csproj'
Ensure-Reference 'src/RetailPOS.Infrastructure/RetailPOS.Infrastructure.csproj' 'src/RetailPOS.Persistence/RetailPOS.Persistence.csproj'
# WPF depends on Application and Infrastructure
Ensure-Reference 'src/RetailPOS.WPF/RetailPOS.WPF.csproj' 'src/RetailPOS.Application/RetailPOS.Application.csproj'
Ensure-Reference 'src/RetailPOS.WPF/RetailPOS.WPF.csproj' 'src/RetailPOS.Infrastructure/RetailPOS.Infrastructure.csproj'
# Tests references
Ensure-Reference 'tests/RetailPOS.Application.Tests/RetailPOS.Application.Tests.csproj' 'src/RetailPOS.Application/RetailPOS.Application.csproj'
Ensure-Reference 'tests/RetailPOS.Infrastructure.Tests/RetailPOS.Infrastructure.Tests.csproj' 'src/RetailPOS.Infrastructure/RetailPOS.Infrastructure.csproj'

Write-Host "Adding NuGet packages (skips when project file missing)."
function Ensure-Package($project, $package) {
	if (-not (Test-Path $project)) { Write-Host "Project $project not found; skipping package $package."; return }
	dotnet add $project package $package | Out-Null
	Write-Host "Ensured package $package for $project"
}

# EF Core + SQLite
Ensure-Package 'src/RetailPOS.Persistence/RetailPOS.Persistence.csproj' 'Microsoft.EntityFrameworkCore.Sqlite'
Ensure-Package 'src/RetailPOS.Persistence/RetailPOS.Persistence.csproj' 'Microsoft.EntityFrameworkCore.Design'

# Dependency injection and hosting
Ensure-Package 'src/RetailPOS.WPF/RetailPOS.WPF.csproj' 'Microsoft.Extensions.Hosting'
Ensure-Package 'src/RetailPOS.WPF/RetailPOS.WPF.csproj' 'Microsoft.Extensions.DependencyInjection'

# MVVM toolkit
Ensure-Package 'src/RetailPOS.WPF/RetailPOS.WPF.csproj' 'CommunityToolkit.Mvvm'

# Validation
Ensure-Package 'src/RetailPOS.Application/RetailPOS.Application.csproj' 'FluentValidation'

# Logging
Ensure-Package 'src/RetailPOS.Infrastructure/RetailPOS.Infrastructure.csproj' 'Serilog'
Ensure-Package 'src/RetailPOS.Infrastructure/RetailPOS.Infrastructure.csproj' 'Serilog.Sinks.File'

# PDF and Excel
Ensure-Package 'src/RetailPOS.Infrastructure/RetailPOS.Infrastructure.csproj' 'QuestPDF'
Ensure-Package 'src/RetailPOS.Infrastructure/RetailPOS.Infrastructure.csproj' 'ClosedXML'

# ESC/POS printing (example)
Ensure-Package 'src/RetailPOS.Infrastructure/RetailPOS.Infrastructure.csproj' 'ESC-POS-USB'

# Testing helpers
Ensure-Package 'tests/RetailPOS.Application.Tests/RetailPOS.Application.Tests.csproj' 'Moq'

# EF tools (global if needed)
if (-not (Get-Command dotnet-ef -ErrorAction SilentlyContinue)) {
	Write-Host "Installing dotnet-ef tool globally."
	dotnet tool install --global dotnet-ef | Out-Null
} else { Write-Host "dotnet-ef already installed (or available)." }

Write-Host "Scaffolding complete. Run 'dotnet restore' and then add initial migrations in Persistence project if desired." -ForegroundColor Green
