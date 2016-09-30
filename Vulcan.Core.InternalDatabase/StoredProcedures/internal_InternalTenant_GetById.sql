CREATE PROCEDURE [internal].[internal_InternalTenant_GetById]
	@tenantId uniqueidentifier
AS
	select [Id], [IsStoreCreated], [IsInitialConfigCompleted], [TempData], [StoreCreateJobKeyHash], [StoreCreateJobKeySalt]
	from [Tenants]
	where [Id]=@tenantId