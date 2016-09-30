CREATE PROCEDURE [aad].[az_AccessCodes_GetByCode]
	@code varchar(256)
AS
	select [ApiUserId], [AccessCode], [ExpiresOn],[IsActive] from [AccessCodes] where [AccessCode]=@code