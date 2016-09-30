CREATE TABLE [auth].[GroupRoles]
(
	[Id] INT NOT NULL identity(100000, 1),
	[GroupId] int not null,
	[RoleId] int not null,
	[CreatedByUserId] int null,
	[CreatedByClientId] int null,
	[CreatedDate] as getdate(),
	[UpdatedByUserId] int null,
	[UpdatedByClientId] int null,
	[UpdatedDate] datetime null,
	constraint PK_GroupRoles_ID primary key ([Id]),
	constraint UN_GroupRoles_GroupIdRoleId unique ([GroupId],[RoleId]),
	constraint FK_GroupRoles_RoleId foreign key ([RoleId]) references [auth].[Roles]([Id]),
	constraint FK_GroupRoles_GroupId foreign key ([GroupId]) references [auth].[Groups]([Id]),
	constraint FK_GroupRoles_CreatedByUserId foreign key ([CreatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_GroupRoles_CreatedByClientId foreign key ([CreatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_GroupRoles_UpdatedByClientId foreign key ([UpdatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_GroupRoles_UpdatedByUserId foreign key ([UpdatedByUserId]) references [auth].[ApiUsers]([Id])
)
