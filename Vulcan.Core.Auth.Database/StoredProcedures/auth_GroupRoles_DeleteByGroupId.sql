CREATE PROCEDURE [auth].[auth_GroupRoles_DeleteByGroupId]
@groupid INT

AS

DELETE [GroupRoles]
WHERE  [GroupId] = @groupid



