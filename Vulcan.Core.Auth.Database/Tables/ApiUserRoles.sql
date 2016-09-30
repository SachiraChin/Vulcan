CREATE TABLE [auth].[ApiUserRoles]
(
	[Id] INT NOT NULL identity(100000, 1),
	[ApiUserId] int not null,
	[RoleId] int not null,
	[CreatedByUserId] int null,
	[CreatedByClientId] int null,
	[CreatedDate] as getdate(),
	[UpdatedByUserId] int null,
	[UpdatedByClientId] int null,
	[UpdatedDate] datetime null,
	constraint PK_ApiUserRoles_ID primary key ([Id]),
	constraint UN_ApiUserRoles_ApiUserIdRoleId unique ([ApiUserId],[RoleId]),
	constraint FK_ApiUserRoles_ApiUserId foreign key ([ApiUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_ApiUserRoles_RoleId foreign key ([RoleId]) references [auth].[Roles]([Id]),
	constraint FK_ApiUserRoles_CreatedByClientId foreign key ([CreatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_ApiUserRoles_CreatedByUserId foreign key ([CreatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_ApiUserRoles_UpdatedByClientId foreign key ([UpdatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_ApiUserRoles_UpdatedByUserId foreign key ([UpdatedByUserId]) references [auth].[ApiUsers]([Id])
)
