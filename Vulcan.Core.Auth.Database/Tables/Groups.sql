CREATE TABLE [auth].[Groups]
(
	[Id] int not null identity(100000, 1),
	[Name] varchar(2048) not null,
	[Description] varchar(4096) null,
	[AudienceId] int not null,
	[CreatedByUserId] int null,
	[CreatedByClientId] int null,
	[CreatedDate] as getdate(),
	[UpdatedByUserId] int null,
	[UpdatedByClientId] int null,
	[UpdatedDate] datetime null,
	[IsDeleted] bit not null default(0),
	[IsSystemId] bit not null,
	constraint PK_Groups_ID primary key ([Id]),
	constraint UN_Groups_Name unique ([Name]),
	constraint FK_Groups_CreatedByUserId foreign key ([CreatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_Groups_CreatedByClientId foreign key ([CreatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_Groups_UpdatedByClientId foreign key ([UpdatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_Groups_UpdatedByUserId foreign key ([UpdatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_Groups_AudienceId foreign key ([AudienceId]) references [auth].[Audiences]([Id])
)
