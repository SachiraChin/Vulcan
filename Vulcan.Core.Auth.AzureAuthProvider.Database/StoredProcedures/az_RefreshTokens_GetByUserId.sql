CREATE PROCEDURE [aad].[az_RefreshTokens_GetByUserId]
	@userId int
AS
	select [UserId], [ExpireIn], [ExpiresOn], [RefreshToken],[AccessToken], [ExternalTenantId] from [RefreshTokens] where [UserId]=@userId