CREATE PROCEDURE [auth].[auth_GroupUsers_GetByApiUserIdGroupId]
	@groupid int,
	@apiuserid int
AS
	SELECT 	[GroupId],
			[ApiUserId],
			[CreatedByUserId],
			[CreatedDate],
			[UpdatedByUserId],
			[UpdatedDate]
	FROM	[GroupUsers]
	WHERE [GroupId]=@groupid AND [ApiUserId]=@apiuserid

