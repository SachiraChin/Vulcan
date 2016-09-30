CREATE PROCEDURE [auth].[auth_ApiUsers_Delete]
	@Id bigint
AS
	update [ApiUsers] set [IsDeleted]=1 where [SystemId]=@Id