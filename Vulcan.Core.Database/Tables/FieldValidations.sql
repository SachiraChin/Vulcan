CREATE TABLE [core].[FieldValidations]
(
	[Id] INT NOT NULL PRIMARY KEY identity(100000,1),
	[FieldId] int not null,
	[FieldName] varchar(64) not null,
	[ValidatorId] uniqueidentifier not null,
	[Message] varchar(max) not null,
	[Data] varchar(max),
	[IsDeleted] bit not null default(0),
	CONSTRAINT FK_FieldValidations_FieldId FOREIGN KEY ([FieldId]) REFERENCES [core].[Fields] ([Id])
)
