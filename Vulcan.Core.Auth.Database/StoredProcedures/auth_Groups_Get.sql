CREATE PROCEDURE [auth].[auth_Groups_Get]
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
	WHERE	[IsDeleted] = 0

