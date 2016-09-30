CREATE TABLE [auth].[TimeZones]
(
	[Id] INT NOT NULL, 
    [Value] VARCHAR(64) NOT NULL, 
    [Abbr] VARCHAR(64) NOT NULL, 
    [Offset] INT NOT NULL, 
    [Isdst] BIT NOT NULL,
	[Text] VARCHAR(128) NOT NULL, 
    constraint PK_TimeZone_ID primary key([Id])
)
