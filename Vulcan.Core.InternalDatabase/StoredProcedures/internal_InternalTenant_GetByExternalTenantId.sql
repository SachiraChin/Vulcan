CREATE PROCEDURE [internal].[internal_InternalTenant_GetByExternalTenantId]
	@ExternalTenantId varchar(256)
AS
	select [IdString]
	from [Tenants] t, [ExternalTenants] et
	where t.Id = et.InternalTenantId and et.ExternalTenantId=@ExternalTenantId
