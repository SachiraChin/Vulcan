CREATE TABLE [aad].[Users]
(
	[Id] INT NOT NULL identity(100000, 1),
	[oid] varchar(36),
	[tid] varchar(36),
	[upn] varchar(512),
	[unique_name] varchar(2048) not null,
	[family_name] varchar(256),
	[given_name] varchar(256),
	constraint PK_Users_ID primary key ([Id]),
	constraint UN_Users_unique_name unique ([unique_name])
)
