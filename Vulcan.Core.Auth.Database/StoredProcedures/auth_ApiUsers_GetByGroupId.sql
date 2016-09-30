CREATE PROCEDURE [auth].[auth_ApiUsers_GetByGroupId]
	@groupid int
AS
	select u.[Id], u.[Username], u.[PasswordHash], u.[PasswordSalt], u.[TokenExpireTimeMinutes],
		u.[CreatedDate], u.[CreatedByUserId], u.[CreatedByClientId], 
		u.[UpdatedByUserId], u.[UpdatedByClientId], u.[UpdatedDate],u. [Type], u.[ExternalRefId],u. [SystemId],u.[ExternalProviderName],
		u.[FirstName], u.[LastName],u. [DisplayName]
	from [ApiUsers] u, [GroupUsers] gu
	where u.[Id] = gu.[ApiUserId] and gu.[GroupId] = @groupid


