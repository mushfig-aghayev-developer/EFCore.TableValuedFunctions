# EF Core Table-Valued Functions

A demonstration of how to use **Table-Valued Functions** with Entity Framework Core and SQL Server.

## Overview

This project shows how to:
- Map a SQL Server inline table-valued function to EF Core
- Call the Table-Valued Functions from C# code using LINQ
- Automatically create the function if it doesn't exist

- ### 3. Set Up the Database

1. Download and restore [AdventureWorks2019](https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver17&tabs=ssms) to your SQL Server
2. Execute the scripts from `sql_scripts/SQl_Procedures.sql`

## Project Structure

```
EFCore.TableValuedFunctions/
├── Database/
│   └── AdventureWorks2019DB.cs    # DbContext with Table-Valued Functions mapping
├── Models/
│   └── SalesOffer.cs              # Entity model for Table-Valued Functions result
├── Sql_Scripts/
│   └── Table_Valued_Functions.sql # SQL script to create Table-Valued Functions manually
├── Program.cs                     # Entry point with usage example
└── EFCore.TableValuedFunctions.csproj
```

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

## The SQL Function

The `ufn_GetSalesInformation` function returns sales order details for a given special offer:

| Column | Type | Description |
|--------|------|-------------|
| CarrierTrackingNumber | nvarchar | Shipping tracking number |
| UnitPrice | money | Price per unit |
| TotalSum | money | Sum of unit prices for the offer |
| Category | nvarchar | Special offer category |
| DiscountPct | smallmoney | Discount percentage |
| SpecialOfferID | int | Special offer identifier |


The application will:
1. Ensure the database exists
2. Create `ufn_GetSalesInformation` if it doesn't exist
3. Query sales data for SpecialOfferID = 7