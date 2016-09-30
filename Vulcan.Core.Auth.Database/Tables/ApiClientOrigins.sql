CREATE TABLE [auth].[ApiClientOrigins]
(
	[Id] INT NOT NULL identity(100000,1),
	[ApiClientId] int not null,
	[Origin] nvarchar(2048) not null,
	[CreatedDate] as getdate(),
	[CreatedByUserId] int null,
	[CreatedByClientId] int null,
	[UpdatedByUserId] int null,
	[UpdatedByClientId] int null,
	[UpdatedDate] datetime null,
	[IsDeleted] bit not null default(0),
	constraint PK_ApiClientOrigins_ID primary key ([Id]),
	constraint FK_ApiClientOrigins_ClientId foreign key ([ApiClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_ApiClientOrigins_CreatedByClientId foreign key ([CreatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_ApiClientOrigins_CreatedByUserId foreign key ([CreatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_ApiClientOrigins_UpdatedByClientId foreign key ([UpdatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_ApiClientOrigins_UpdatedByUserId foreign key ([UpdatedByUserId]) references [auth].[ApiUsers]([Id])
)
