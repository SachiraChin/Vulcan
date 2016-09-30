CREATE PROCEDURE [auth].[auth_GroupUsers_ApiUserId]
	@ApiUserId int
AS
	SELECT 	[GroupId],
			[ApiUserId],
			[CreatedByUserId],
			[CreatedByClientId],
			[CreatedDate],
			[UpdatedByUserId],
			[UpdatedByClientId],
			[UpdatedDate]
	FROM	[GroupUsers]
	WHERE	[ApiUserId]=@ApiUserId

