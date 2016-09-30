CREATE TABLE [auth].[Audiences]
(
	[Id] INT NOT NULL identity(100000,1),
	[AudienceId] varchar(32) not null,
	[Secret] varchar(80) not null,
	[Name] varchar(64) not null,
	[CreatedDate] as getdate(),
	[CreatedByUserId] int null,
	[CreatedByClientId] int null,
	[UpdatedByUserId] int null,
	[UpdatedByClientId] int null,
	[UpdatedDate] datetime null,
	constraint PK_Audiences_ID primary key ([Id]),
	constraint UN_Audiences_AudienceId unique ([AudienceId]),
	constraint FK_Audiences_CreatedByClientId foreign key ([CreatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_Audiences_CreatedByUserId foreign key ([CreatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_Audiences_UpdatedByClientId foreign key ([UpdatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_Audiences_UpdatedByUserId foreign key ([UpdatedByUserId]) references [auth].[ApiUsers]([Id])
)
