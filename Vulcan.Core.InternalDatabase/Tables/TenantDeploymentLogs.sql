CREATE TABLE [internal].[TenantDeploymentLogs]
(
	[Id] INT NOT NULL PRIMARY KEY identity(10000, 1),
	[TenantId] uniqueidentifier not null,
	[LogEntry] nvarchar(max),
	[CreatedDate] as getdate(),
	constraint FK_TenantDeploymentLogs_TenantId foreign key ([TenantId]) references [internal].[Tenants]([Id])
)
