CREATE PROCEDURE [auth].[auth_Groups_GetByName]
	@name varchar(2048)
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
	WHERE	[Name]=@name and [IsDeleted]=0
