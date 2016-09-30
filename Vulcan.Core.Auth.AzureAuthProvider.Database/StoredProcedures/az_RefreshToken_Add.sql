CREATE PROCEDURE [aad].[az_RefreshToken_Add]
	@UserId int,
	@ExpireIn int,
	@ExpiresOn datetime,
	@RefreshToken varchar(max),
	@AccessToken varchar(max),
	@ExternalTenantId varchar(256)
AS
	insert into [RefreshTokens] ([UserId], [ExpireIn], [ExpiresOn], [RefreshToken], [AccessToken], [ExternalTenantId])
	values (@UserId, @ExpireIn, @ExpiresOn, @RefreshToken,@AccessToken,@ExternalTenantId)
	