CREATE PROCEDURE [auth].[auth_Groups_GetByUserSystemId]
	@SystemId bigint
AS
begin

	SELECT	g.[Id], 
			g.[Name],
			g.[Description],
			g.[AudienceId],
			g.[CreatedByUserId],
			g.[CreatedByClientId],
			g.[CreatedDate],
			g.[UpdatedByUserId],
			g.[UpdatedByClientId],
			g.[UpdatedDate],
			g.[IsDeleted],
			g.[IsSystemId]
	FROM	[Groups] g
		left join [GroupUsers] gu on gu.[GroupId]=g.[Id]
		left join [ApiUsers] u on u.[Id]=gu.[ApiUserId]
	WHERE	g.[IsDeleted] = 0 and u.[SystemId]=@SystemId

end