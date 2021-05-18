CREATE PROCEDURE [dbo].[spUserLookup]
	@Id nvarchar(128)
AS
BEGIN
	set nocount on;

	select Id,FirstName,LastName,EmailAddress,CreatedDate
	from [dbo].[User]
	where Id=@Id
END
