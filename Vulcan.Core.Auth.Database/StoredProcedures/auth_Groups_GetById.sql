CREATE PROCEDURE [auth].[auth_Groups_GetById]
	@id int
AS
	SELECT	[Id], 
			[Name],
			[Description],
			[AudienceId],
			[CreatedByUserId],
			[CreatedByClientId],
			[CreatedDate],
			[UpdatedByUserId],
			[UpdatedByClientId],
			[UpdatedDate],
			[IsDeleted],
			[IsSystemId]
	FROM	[Groups]
	WHERE	[Id]=@id and [IsDeleted]=0
