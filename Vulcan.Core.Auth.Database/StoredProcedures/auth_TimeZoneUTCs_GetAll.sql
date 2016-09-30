CREATE PROCEDURE [auth].[auth_TimeZoneUTCs_GetAll]
	
AS
	SELECT [Id],[TimezoneId],[Utc]
	FROM [TimeZoneUTCs]
