CREATE PROCEDURE [auth].[auth_RefreshTokens_DeleteByTokenHash]
	@tokenHash varchar(64)
AS
	delete from [RefreshTokens] where [TokenHash]=@tokenHash
