# EF Core Table-Valued Functions

A demonstration of how to use **Table-Valued Functions** with Entity Framework Core and SQL Server.

## Overview

This project shows how to:
- Map a SQL Server inline table-valued function to EF Core
- Call the Table-Valued Functions from C# code using LINQ
- Automatically create the function if it doesn't exist

## Set Up the Database

1. Download and restore [AdventureWorks2019](https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver17&tabs=ssms) to your SQL Server
2. Execute the scripts from `Sql_Scripts/Table_Valued_Functions.sql`

## Configuration

Update the connection string in `AdventureWorks2019DB.cs` if needed:

```csharp
optionsBuilder.UseSqlServer(
    @"Data Source=.;Initial Catalog=AdventureWorks2019;Integrated Security=SSPI;TrustServerCertificate=True");
```

## How It Works

### 1. Define the Table-Valued Functions in DbContext

```csharp
public IQueryable<SalesOffer> ufn_GetSalesInformation(int specialOfferId) 
    => FromExpression(() => ufn_GetSalesInformation(specialOfferId));
```

### 2. Register the Table-Valued Functions in OnModelCreating

```csharp
modelBuilder.HasDbFunction(
    typeof(AdventureWorks2019DB).GetMethod(nameof(ufn_GetSalesInformation), new[] { typeof(int) })!)
    .HasName("ufn_GetSalesInformation")
    .HasSchema("dbo");
```

### 3. Call the Table-Valued Functions

```csharp
var salesOffers = await dbContext.ufn_GetSalesInformation(7).ToListAsync();
```