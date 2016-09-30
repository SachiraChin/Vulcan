CREATE PROCEDURE [auth].[auth_GroupUsers_DeleteByGroupId]
@groupid INT
AS
DELETE [GroupUsers]
WHERE  [GroupId] = @groupid

GO
