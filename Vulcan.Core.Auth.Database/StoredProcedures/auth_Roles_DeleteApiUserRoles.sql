CREATE PROCEDURE [auth].[auth_Roles_DeleteApiUserRoles]
	@userId int = 0,
	@Ids [IntList] READONLY
AS
	delete from [ApiUserRoles] where [ApiUserId]=@userId and [RoleId] in (select [Value] from @Ids)
