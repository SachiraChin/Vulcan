CREATE PROCEDURE [aad].[az_AccessCodes_Update]
	@ApiUserId int,
	@AccessCode varchar(max),
	@ExpiresOn datetime
AS
	delete [AccessCodes] where [ApiUserId]=@ApiUserId
	
	insert into [AccessCodes]([ApiUserId], [AccessCode], [ExpiresOn]) values(@ApiUserId, @AccessCode,@ExpiresOn)