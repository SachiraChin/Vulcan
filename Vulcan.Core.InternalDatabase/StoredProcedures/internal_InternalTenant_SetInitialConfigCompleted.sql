CREATE PROCEDURE [internal].[internal_InternalTenant_SetInitialConfigCompleted]
	@tenantId uniqueidentifier
AS
	update [Tenants] set [IsInitialConfigCompleted]=1 where [Id]=@tenantId
