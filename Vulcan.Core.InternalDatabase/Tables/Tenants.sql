CREATE TABLE [internal].[Tenants]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY default(newid()),
	[IdString] as REPLACE(CAST([Id] AS VARCHAR(36)),'-',''),
	[IsStoreCreated] bit not null default(0),
	[IsInitialConfigCompleted] bit not null default(0),
	[TempData] varchar(max) null,
	[StoreCreateJobKeyHash] varchar(max),
	[StoreCreateJobKeySalt] varchar(max)
)
