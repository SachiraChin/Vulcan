CREATE TABLE [auth].[GroupUsers]
(
	[Id] int not null identity(100000, 1),
	[GroupId] int not null,
	[ApiUserId] int not null,
	[CreatedByUserId] int null,
	[CreatedDate] as getdate(),
	[CreatedByClientId] int null,
	[UpdatedByUserId] int null,
	[UpdatedByClientId] int null,
	[UpdatedDate] datetime null,
	constraint PK_GroupUsers_ID primary key ([Id]),
	constraint UN_GroupUsers_ID unique ([GroupId],[ApiUserId]),
	constraint FK_GroupUsers_ApiUserId foreign key ([ApiUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_GroupUsers_GroupId foreign key ([GroupId]) references [auth].[Groups]([Id]),
	constraint FK_GroupUsers_CreatedByUserId foreign key ([CreatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_GroupUsers_CreatedByClientId foreign key ([CreatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_GroupUsers_UpdatedByClientId foreign key ([UpdatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_GroupUsers_UpdatedByUserId foreign key ([UpdatedByUserId]) references [auth].[ApiUsers]([Id])
)
