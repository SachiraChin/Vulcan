CREATE TABLE [auth].[RefreshTokens]
(
	[Id] INT NOT NULL identity(100000,1),
	[TokenHash] varchar(64) not null,
	[Subject] varchar(32) not null,
	[IssuedDate] datetime not null,
	[ExpiryDate] datetime not null,
	[Ticket] varchar(max) not null,
	constraint PK_RefreshTokens_ID primary key ([Id]),
	constraint UN_RefreshTokens_TokenHash unique ([TokenHash])
)
