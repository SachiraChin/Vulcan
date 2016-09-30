CREATE TABLE [auth].[ApiClientRoles]
(
	[Id] INT NOT NULL identity(100000, 1),
	[ApiClientId] int not null,
	[RoleId] int not null,
	[CreatedByUserId] int null,
	[CreatedByClientId] int null,
	[CreatedDate] as getdate(),
	[UpdatedByUserId] int null,
	[UpdatedByClientId] int null,
	[UpdatedDate] datetime null,
	constraint PK_ApiClientRoles_ID primary key ([Id]),
	constraint FK_ApiClientRoles_ApiClientId foreign key ([ApiClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_ApiClientRoles_RoleId foreign key ([RoleId]) references [auth].[Roles]([Id]),
	constraint FK_ApiClientRoles_CreatedByClientId foreign key ([CreatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_ApiClientRoles_CreatedByUserId foreign key ([CreatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_ApiClientRoles_UpdatedByClientId foreign key ([UpdatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_ApiClientRoles_UpdatedByUserId foreign key ([UpdatedByUserId]) references [auth].[ApiUsers]([Id])
)
