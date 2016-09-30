CREATE TABLE [core].[Migrations]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY,
	[CreatedDate] datetime not null default(getdate()),
	[IsCompleted] bit not null default(0)
)
