CREATE TYPE [core].[Validations] AS TABLE
(
	[Id] int,
	[ValidatorId] uniqueidentifier not null,
	[Message] varchar(max) not null,
	[Data] varchar(max)
)
