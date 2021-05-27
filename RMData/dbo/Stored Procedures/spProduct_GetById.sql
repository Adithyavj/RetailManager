CREATE PROCEDURE [dbo].[spProduct_GetById]
	@id int = 0
AS
BEGIN
	set NOCOUNT ON;

	select Id, ProductName, [Description], RetailPrice, QuantityInStock, IsTaxable
	from dbo.Product 
	where Id = @id
END
