CREATE TABLE [core].[FieldConstraints]
(
	[Id] INT NOT NULL PRIMARY KEY identity(100000,1),
	[FieldId] int not null,
	[FieldName] varchar(64) not null,
	[ConstraintProviderId] uniqueidentifier not null,
	[Data] nvarchar(max),
	[ConstraintName] varchar(128),
	CONSTRAINT FK_FieldConstraints_FieldId FOREIGN KEY ([FieldId]) REFERENCES [core].[Fields] ([Id])
)
