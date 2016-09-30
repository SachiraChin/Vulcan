CREATE TABLE [aad].[AccessCodes]
(
	[ApiUserId] bigint NOT NULL PRIMARY KEY,
	[AccessCode] varchar(max),
	[ExpiresOn] datetime,
	[IsActive] bit not null default(1)
)
