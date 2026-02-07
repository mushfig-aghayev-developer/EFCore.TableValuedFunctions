using EFCore.TableValuedFunctions.Database;
using EFCore.TableValuedFunctions.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EFCore.TableValuedFunctions
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var dbContext = new AdventureWorks2019DB())
            {
                dbContext.Database.EnsureCreated();

                // You can implement the function in the database using raw SQL, and then call it from EF Core.
                // Implement in an empty migration, in the OnModelCreating method of your DbContext, or directly in your code as shown below.
                // If ufn_GetSalesInformation does not exist, create it:
                await dbContext.Database.ExecuteSqlRawAsync(
                    @"IF NOT EXISTS (
                    SELECT 1 
                    FROM sys.objects 
                    WHERE object_id = OBJECT_ID(N'dbo.ufn_GetSalesInformation') 
                      AND type = N'IF'  -- IF = Inline table-valued function
                )
                BEGIN
                    EXEC('
                        CREATE FUNCTION dbo.ufn_GetSalesInformation(@specialOfferId AS INT)
                        RETURNS TABLE AS RETURN
                        SELECT 
                            sd.CarrierTrackingNumber, 
                            sd.UnitPrice, 
                            SUM(sd.UnitPrice) OVER (PARTITION BY sd.SpecialOfferId) AS TotalSum,
                            sf.Category, 
                            sf.DiscountPct, 
                            sd.SpecialOfferID
                        FROM Sales.SalesOrderDetail AS sd
                        INNER JOIN Sales.SpecialOffer AS sf
                            ON sf.SpecialOfferID = sd.SpecialOfferID
                        WHERE sf.SpecialOfferID = @specialOfferId
                    ');
                    PRINT 'Function ufn_GetSalesInformation created successfully.';
                END
                ELSE
                BEGIN
                    PRINT 'Function ufn_GetSalesInformation already exists.';
                END");

                var salesOffers = await dbContext.ufn_GetSalesInformation(7).ToListAsync();
            }
            Console.ReadLine();
        }
    }
}