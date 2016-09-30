CREATE PROCEDURE [aad].[az_AccessCodes_Add]
	@ApiUserId int,
	@AccessCode varchar(max),
	@ExpiresOn datetime
AS
	insert into [AccessCodes]([ApiUserId], [AccessCode], [ExpiresOn]) values(@ApiUserId, @AccessCode,@ExpiresOn)
