CREATE PROCEDURE [internal].[internal_InternalTenant_SetTempData]
	@tenantId uniqueidentifier,
	@tempData varchar(max)
AS
	update [Tenants] set [TempData]=@tempData where [Id]=@tenantId