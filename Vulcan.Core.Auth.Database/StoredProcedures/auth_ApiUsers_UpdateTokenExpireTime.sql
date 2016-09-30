CREATE PROCEDURE [auth].[auth_ApiUsers_UpdateTokenExpireTime]
	@UserSystemId bigint,
	@TokenExpireTimeMinutes int
AS
	update [ApiUsers] set [TokenExpireTimeMinutes]=@TokenExpireTimeMinutes
	where [SystemId] = @UserSystemId