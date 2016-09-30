CREATE PROCEDURE [auth].[auth_Roles_Get]
	@isSystem bit
AS
	if @isSystem = 1
		select r.[Id], r.[Name], r.[Title], r.[IsHidden], r.[Type],
			r.[CreatedDate], r.[CreatedByUserId], r.[CreatedByClientId], 
			r.[UpdatedByUserId], r.[UpdatedByClientId], r.[UpdatedDate], r.[AudienceId]
		from [Roles] r
	else
		select r.[Id], r.[Name], r.[Title], r.[IsHidden], r.[Type],
			r.[CreatedDate], r.[CreatedByUserId], r.[CreatedByClientId], 
			r.[UpdatedByUserId], r.[UpdatedByClientId], r.[UpdatedDate], r.[AudienceId]
		from [Roles] r
		where [IsHidden] = 0 and [Type]<>99