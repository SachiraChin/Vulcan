CREATE PROCEDURE [auth].[auth_RefreshTokens_GetByTokenHash]
	@tokenHash varchar(64)
AS
	SELECT [TokenHash], [Subject], [IssuedDate], [ExpiryDate], [Ticket] from [RefreshTokens] where [TokenHash]=@tokenHash
