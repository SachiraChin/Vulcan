CREATE PROCEDURE [auth].[auth_ApiUsers_Search]
	@SearchText nvarchar(256)
AS
begin
	
	declare @SearchCondition nvarchar(258)
	set @SearchCondition = N'%' + @SearchText+ N'%'

	select [Id], [Username], [PasswordHash], [PasswordSalt], [TokenExpireTimeMinutes],
		[CreatedDate], [CreatedByUserId], [CreatedByClientId], 
		[UpdatedByUserId], [UpdatedByClientId], [UpdatedDate], [Type], [ExternalRefId], [SystemId],[ExternalProviderName],
		[FirstName], [LastName], [DisplayName]
	from [ApiUsers]
	where [Username] like @SearchCondition or  [FirstName] like @SearchCondition or  [LastName] like @SearchCondition or  [DisplayName] like @SearchCondition

end