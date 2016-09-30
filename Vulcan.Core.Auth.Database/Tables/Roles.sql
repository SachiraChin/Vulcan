CREATE TABLE [auth].[Roles]
(
	[Id] INT NOT NULL identity(100000, 1),
	[Name] varchar(64) not null,
	[Title] varchar(64) not null,
	[IsHidden] bit not null default(0),
	[Type] tinyint not null,
	[CreatedByUserId] int null,
	[CreatedByClientId] int null,
	[CreatedDate] as getdate(),
	[UpdatedByUserId] int null,
	[UpdatedByClientId] int null,
	[UpdatedDate] datetime null,
	[AudienceId] int not null,
	constraint PK_Roles_ID primary key ([Id]),
	constraint UN_Roles_Name unique ([Name]),
	constraint FK_Roles_CreatedByClientId foreign key ([CreatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_Roles_CreatedByUserId foreign key ([CreatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_Roles_UpdatedByClientId foreign key ([UpdatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_Roles_UpdatedByUserId foreign key ([UpdatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_Roles_AudienceId foreign key ([AudienceId]) references [auth].[Audiences]([Id])
)
