CREATE PROCEDURE [internal].[internal_InternalTenant_SetStoreCreateJobKey]
	@tenantId uniqueidentifier,
	@hash varchar(max),
	@salt varchar(max)
AS
	update [Tenants] set [StoreCreateJobKeyHash]=@hash, [StoreCreateJobKeySalt]=@salt where [Id]=@tenantId