CREATE PROCEDURE [internal].[internal_TenantDeploymentLogs_Add]
	@tenantId uniqueidentifier,
	@logEntry nvarchar(max)
AS
	insert into [TenantDeploymentLogs]([TenantId], [LogEntry])
	values (@tenantId, @logEntry)
