﻿CREATE PROCEDURE [dbo].[spSale_Insert]
	@Id int output,
	@CashierId nvarchar(128),
	@SaleDate datetime2,
	@SubTotal money,
	@Tax money,
	@Total money
AS
BEGIN
	set nocount ON;
	
	insert into dbo.Sale(CashierId, SaleDate, SubTotal, Tax, Total)
	values (@CashierId, @SaleDate, @SubTotal, @Tax, @Total)

	select @Id = @@IDENTITY;

END
