CREATE PROCEDURE [auth].[auth_Groups_GetByAudienceId]
	@AudienceId varchar(32)
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
	WHERE	[AudienceId]=@AudienceId and [IsDeleted]=0