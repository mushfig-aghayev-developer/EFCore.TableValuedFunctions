USE AdventureWorks2019;
GO

IF NOT EXISTS (
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
END
GO

-- Test the function
-- SELECT * FROM Sales.SpecialOffer AS sf;
SELECT * FROM dbo.ufn_GetSalesInformation(7);

-- To drop: DROP FUNCTION dbo.ufn_GetSalesInformation;