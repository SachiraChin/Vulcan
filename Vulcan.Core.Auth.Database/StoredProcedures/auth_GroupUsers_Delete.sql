CREATE PROCEDURE [auth].[auth_GroupUsers_Delete]
	@groupid int,
	@Ids [IntList] READONLY

AS
	delete from [GroupUsers] where [GroupId]=@groupid and [ApiUserId] in (select[Value] from @Ids)
