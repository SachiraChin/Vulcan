CREATE TABLE [core].[MigrationEntries]
(
	[Id] INT NOT NULL PRIMARY KEY identity(1,1),
	[CreatedDate] datetime not null default(getdate()),
	[MigrationId] uniqueidentifier not null,
	[TableName] varchar(64) not null,
	[EntryJson] nvarchar(max) not null,
	[ExecutionOrderIndex] int not null,
	CONSTRAINT FK_MigrationId FOREIGN KEY ([MigrationId]) REFERENCES [core].[Migrations] ([Id])
)
