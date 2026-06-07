# RetailPOS - Retail Billing & Inventory Management (MVP)

This repository contains an enterprise-inspired MVP for a Retail Billing & Inventory Management System built with WPF and .NET 8 using Clean Architecture principles.

## Phase 1 - Solution Creation
Run the included PowerShell script to scaffold the solution and projects:

PowerShell:

```powershell
./create_solution.ps1
```

Or run the commands manually. The script will:
- Create `RetailPOS.sln` and the projects under `src/` and `tests/`.
- Add project references following Clean Architecture.
- Install NuGet packages for EF Core, FluentValidation, CommunityToolkit.Mvvm, Serilog, QuestPDF, ClosedXML, and more.

## Architecture Decisions
- Clean Architecture: `RetailPOS.Domain` is pure, `RetailPOS.Application` contains business logic and interfaces, `RetailPOS.Persistence` implements EF Core, `RetailPOS.Infrastructure` contains cross-cutting concerns (logging, PDF/Excel, printing), and `RetailPOS.WPF` is the UI.
- Dependency rule: inner layers do not depend on outer layers.
- Use EF Core (SQLite) with code-first migrations for MVP.
- MVVM pattern using CommunityToolkit.Mvvm for lightweight, testable view models.
- Use Serilog for structured logging.
- Use FluentValidation for input validation.

## Next Steps (Phase 2)
- Implement Domain entities and value objects in `src/RetailPOS.Domain`.
- Define repository interfaces in `src/RetailPOS.Application`.

Refer to the `create_solution.ps1` script for exact CLI commands.
