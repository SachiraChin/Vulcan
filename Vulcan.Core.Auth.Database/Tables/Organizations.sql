CREATE TABLE [auth].[Organizations]
(
	[Id] INT NOT NULL identity(100,1), 
    [Name] VARCHAR(2048) NOT NULL, 
    [Address] VARCHAR(4096) NOT NULL, 
    [Email] VARCHAR(50) NOT NULL, 
    [Timezone] VARCHAR(50) NOT NULL, 
	[CreatedDate] as getdate(),
	[CreatedByUserId] int null,
	[CreatedByClientId] int null,
	[UpdatedByUserId] int null,
	[UpdatedByClientId] int null,
	[UpdatedDate] datetime null,
    constraint PK_Oraganizations_ID primary key([Id]),
	constraint FK_Oraganizations_CreatedByUserId foreign key ([CreatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint FK_Organizations_UpdatedByUserId foreign key([UpdatedByUserId]) references [auth].[ApiUsers]([Id]),
	constraint Fk_Organizations_CreatedByClientId foreign key ([CreatedByClientId]) references [auth].[ApiClients]([Id]),
	constraint FK_Organizations_UpdatedByClientId foreign key ([UpdatedByClientId]) references [auth].[ApiClients]([Id])

)
