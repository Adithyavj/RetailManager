CREATE PROCEDURE [dbo].[spInventory_GetAll]
AS
BEGIN
	set nocount ON;

	select [ProductId], [Quantity], [PurchasePrice], [PurchaseDate]
	from dbo.Inventory
END
