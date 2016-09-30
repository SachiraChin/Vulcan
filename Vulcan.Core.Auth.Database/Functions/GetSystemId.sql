CREATE FUNCTION [auth].[GetSystemId]
(
)
RETURNS bigint 
begin
	declare @newId uniqueidentifier

	select @newId=[Id] from [NewId]
	RETURN cast(Datediff(s, '1970-01-01', GETUTCDATE()) AS bigint) + (ABS(CHECKSUM(@newId)) % 1000000000)
end