CREATE PROCEDURE [auth].[auth_ApiUsers_GetByUsername]
	@username varchar(2048)
AS
	select [Id], [Username], [PasswordHash], [PasswordSalt], [TokenExpireTimeMinutes],
		[CreatedDate], [CreatedByUserId], [CreatedByClientId], 
		[UpdatedByUserId], [UpdatedByClientId], [UpdatedDate], [Type], [ExternalRefId], [SystemId],[ExternalProviderName],
		[FirstName], [LastName], [DisplayName]
	from [ApiUsers]
	where [Username] = @username
