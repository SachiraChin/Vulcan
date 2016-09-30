CREATE PROCEDURE [auth].[auth_Roles_GetByGroupId]
	@groupid int
AS
	select r.[Id], r.[Name], r.[Title], r.[IsHidden], r.[Type],
		r.[CreatedDate], r.[CreatedByUserId], r.[CreatedByClientId], 
		r.[UpdatedByUserId], r.[UpdatedByClientId], r.[UpdatedDate], r.[AudienceId]
	from [Roles] r, [GroupRoles] gr
	where r.[Id] = gr.[RoleId] and gr.[GroupId] = @groupid

