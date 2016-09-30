CREATE PROCEDURE [internal].[internal_InternalTenant_SetStoreCreated]
	@tenantId uniqueidentifier
AS
	update [Tenants] set [IsStoreCreated]=1 where [Id]=@tenantId
