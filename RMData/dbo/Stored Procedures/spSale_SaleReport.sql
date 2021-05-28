CREATE PROCEDURE [dbo].[spSale_SaleReport]
AS
BEGIN
	set nocount ON;

	select [s].[SaleDate], [s].[SubTotal], [s].[Tax], [s].[Total], [u].[FirstName], [u].[LastName], [u].[EmailAddress]
	from dbo.Sale s
	inner join dbo.[User] u on s.CashierId = u.Id
END
