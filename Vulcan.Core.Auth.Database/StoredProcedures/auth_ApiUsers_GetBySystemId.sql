CREATE PROCEDURE [auth].[auth_ApiUsers_GetBySystemId]
	@id bigint
AS
	select [Id], [Username], [PasswordHash], [PasswordSalt], [TokenExpireTimeMinutes],
		[CreatedDate], [CreatedByUserId], [CreatedByClientId], 
		[UpdatedByUserId], [UpdatedByClientId], [UpdatedDate], [Type], [ExternalRefId], [SystemId],[ExternalProviderName],
		[FirstName], [LastName], [DisplayName]
	from [ApiUsers]
	where [SystemId]=@id
