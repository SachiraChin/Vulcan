CREATE TABLE [auth].[Audits]
(
	[Id] INT NOT NULL identity(100000, 1),
	[Type] char(1) not null,
	[TableName] varchar(64) not null,
	[PrimaryKeyField] varchar(64) not null,
	[PrimaryKeyValue] int not null,
	[FieldName] varchar(64) not null,
	[OldValue] nvarchar(max) null,
	[NewValue] nvarchar(max) null,
	[CreatedByUserId] int null,
	[CreatedByClientId] int null,
	[CreatedDate] as getdate(),
	constraint PK_Audits_ID primary key ([Id]),
	constraint FK_Audits_CreatedByClientId foreign key ([CreatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_Audits_CreatedByUserId foreign key ([CreatedByUserId]) references [auth].[ApiUsers]([Id])
)
