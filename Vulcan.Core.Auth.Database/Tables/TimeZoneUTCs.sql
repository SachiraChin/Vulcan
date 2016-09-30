CREATE TABLE [auth].[TimeZoneUTCs]
(
	[Id] INT NOT NULL identity(100,1), 
    [TimezoneId] INT NOT NULL, 
    [Utc] VARCHAR(512) NOT NULL,
	constraint PK_TimeZoneUTCs_ID primary key([Id]),
	constraint FK_TimeZones_ID foreign key([TimezoneId]) references [auth].[TimeZones]([Id])
	 
)
