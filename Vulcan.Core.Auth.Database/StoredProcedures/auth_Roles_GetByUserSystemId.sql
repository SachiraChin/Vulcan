CREATE PROCEDURE [auth].[auth_Roles_GetByUserSystemId]
	@userSystemId bigint
AS
	select r.[Id], r.[Name], r.[Title], r.[IsHidden], r.[Type],
		r.[CreatedDate], r.[CreatedByUserId], r.[CreatedByClientId], 
		r.[UpdatedByUserId], r.[UpdatedByClientId], r.[UpdatedDate]
	from [Roles] r, [ApiUserRoles] acr, [ApiUsers] au
	where r.[Id] = acr.[RoleId] and acr.[ApiUserId] = au.[Id] and au.[SystemId]=@userSystemId
