CREATE TABLE [internal].[ExternalTenants]
(
	[Id] INT NOT NULL PRIMARY KEY identity(100000, 1),
	[ExternalTenantId] varchar(256),
	[InternalTenantId] uniqueidentifier not null,
	constraint FK_ExternalTenants_InternalTenantId foreign key ([InternalTenantId]) references [internal].[Tenants]([Id]),
)
