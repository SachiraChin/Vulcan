CREATE PROCEDURE [auth].[auth_Roles_GetByUsername]
	@username varchar(2048)
AS
	select r.[Id], r.[Name], r.[Title], r.[IsHidden], r.[Type],
		r.[CreatedDate], r.[CreatedByUserId], r.[CreatedByClientId], 
		r.[UpdatedByUserId], r.[UpdatedByClientId], r.[UpdatedDate], r.[AudienceId]
	from [Roles] r, [ApiUserRoles] acr, [ApiUsers] ac
	where r.[Id] = acr.[RoleId] and acr.[ApiUserId] = ac.[Id] and ac.[Username] = @username
