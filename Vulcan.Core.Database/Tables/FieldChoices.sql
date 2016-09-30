CREATE TABLE [core].[FieldChoices]
(
	[Id] INT NOT NULL PRIMARY KEY identity(100000,1),
	[FieldId] int not null,
	[Text] nvarchar(255) not null,
	[IsDeleted] bit not null default(0),
	CONSTRAINT FK_FieldChoices_FieldId FOREIGN KEY ([FieldId]) REFERENCES [core].[Fields] ([Id])
)
