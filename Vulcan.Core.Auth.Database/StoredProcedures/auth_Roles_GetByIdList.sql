CREATE PROCEDURE [auth].[auth_Roles_GetByIdList]
	@Ids [IntList] READONLY
AS
	select r.[Id], r.[Name], r.[Title], r.[IsHidden], r.[Type],
		r.[CreatedDate], r.[CreatedByUserId], r.[CreatedByClientId], 
		r.[UpdatedByUserId], r.[UpdatedByClientId], r.[UpdatedDate], r.[AudienceId]
	from [Roles] r
	where r.[Id] in (select [Value] from @Ids)