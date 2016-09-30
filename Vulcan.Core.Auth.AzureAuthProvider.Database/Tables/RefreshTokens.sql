CREATE TABLE [aad].[RefreshTokens]
(
	--[Id] INT NOT NULL identity(100000, 1),
	[UserId] int not null,
	[ExpireIn] int not null,
	[ExpiresOn] datetime not null,
	[RefreshToken] varchar(max) not null,
	[AccessToken] varchar(max) not null,
	[ExternalTenantId] varchar(256) not null,
	constraint PK_RefreshTokens_UserId primary key ([UserId]),
	constraint FK_RefreshTokens_UserId foreign key ([UserId]) references [aad].[Users]([Id])
)
