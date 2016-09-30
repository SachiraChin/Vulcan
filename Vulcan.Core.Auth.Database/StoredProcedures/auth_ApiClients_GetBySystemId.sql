CREATE PROCEDURE [auth].[auth_ApiClients_GetBySystemId]
	@id bigint
AS
	select [Id], [ClientId], [ClientSecretHash], [ClientSecretSalt], [Type], [TokenExpireTimeMinutes], 
		[CreatedDate], [CreatedByUserId], [CreatedByClientId], 
		[UpdatedByUserId], [UpdatedByClientId], [UpdatedDate],[SystemId]
	from [ApiClients]
	where [SystemId] = @id and [IsDeleted]=0
