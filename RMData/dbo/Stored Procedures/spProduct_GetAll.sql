CREATE PROCEDURE [dbo].[spProduct_GetAll]
AS
BEGIN
	set NOCOUNT ON;

	select Id, ProductName, [Description], RetailPrice, QuantityInStock, IsTaxable
	from dbo.Product 
	order by ProductName
END
