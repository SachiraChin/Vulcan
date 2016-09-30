CREATE PROCEDURE [internal].[internal_ExternalTenants_Add]
	@ExternalTenantId varchar(256)
AS
begin
	declare @InternalTenantId uniqueidentifier
	set @InternalTenantId = newid()

	INSERT INTO [Tenants] ([Id]) VALUES (@InternalTenantId)
	INSERT INTO [ExternalTenants] ([ExternalTenantId],[InternalTenantId]) VALUES (@ExternalTenantId,@InternalTenantId)

	select REPLACE(CAST(@InternalTenantId AS VARCHAR(36)),'-','')
end