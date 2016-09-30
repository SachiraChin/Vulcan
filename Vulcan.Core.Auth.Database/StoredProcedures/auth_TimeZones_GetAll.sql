CREATE PROCEDURE [auth].[auth_TimeZones_GetAll]

AS
	select [Id], [Value], [Abbr],[Isdst], [Offset], [Text]
	from [TimeZones]

