# Matchbook.Server

Back-end solution for Matchbook

Core tools, frameworks and components:
 - .NET Core 3.0
 - ASP.NET Core 3.0 WebApi
 - Entity Framework Core 3.0

## Entity Framework Core

[EF Core overview](https://docs.microsoft.com/en-us/ef/core/).

DB schema is managed using [EF Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/) mechanism.

To get EF Tools running and update the database:

```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef database update
```
