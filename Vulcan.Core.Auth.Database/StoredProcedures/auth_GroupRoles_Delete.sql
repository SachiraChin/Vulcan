CREATE PROCEDURE [auth].[auth_GroupRoles_Delete]
	@groupid int,
	@Ids [IntList] READONLY

AS
	delete from [GroupRoles] where [GroupId]=@groupid and [RoleId] in (select[Value] from @Ids)
