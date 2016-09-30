CREATE PROCEDURE [auth].[auth_ApiClients_Delete]
	@id bigint
AS
	update [ApiClients] set [IsDeleted]=0 where [SystemId]=@id
