CREATE PROCEDURE [aad].[az_RefreshTokens_Update]
	@UserId int,
	@ExpireIn int,
	@ExpiresOn datetime,
	@RefreshToken varchar(max),
	@AccessToken varchar(max),
	@ExternalTenantId varchar(256)
AS
	delete from [RefreshTokens] 
	where [UserId]=@UserId

	insert into [RefreshTokens] ([UserId], [ExpireIn], [ExpiresOn], [RefreshToken], [AccessToken], [ExternalTenantId])
	values (@UserId, @ExpireIn, @ExpiresOn, @RefreshToken,@AccessToken,@ExternalTenantId)
	
