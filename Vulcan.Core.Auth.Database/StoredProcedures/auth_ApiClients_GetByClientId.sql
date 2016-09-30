CREATE PROCEDURE [auth].[auth_ApiClients_GetByClientId]
	@clientId varchar(32)
AS
	select [Id], [ClientId], [ClientSecretHash], [ClientSecretSalt], [Type], [TokenExpireTimeMinutes], 
		[CreatedDate], [CreatedByUserId], [CreatedByClientId], 
		[UpdatedByUserId], [UpdatedByClientId], [UpdatedDate],[SystemId]
	from [ApiClients]
	where [ClientId] = @clientId and [IsDeleted]=0
