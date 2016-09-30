CREATE PROCEDURE [auth].[auth_Roles_GetByUsernameAudienceId]
	@Username varchar(2048),
	@AudienceId varchar(32)
AS
	declare @ApiUserId int
	declare @InternalAudienceId int

	select @InternalAudienceId=[Id] from [Audiences] where [AudienceId]=@AudienceId

	select @ApiUserId=[Id] from [ApiUsers] where [Username] =@Username
	
	DECLARE @RoleIds AS TABLE(Id int)

	insert into @RoleIds
	select gr.[RoleId]
	from [GroupRoles] gr
		left join [Groups] g on gr.[GroupId]=  g.[Id]
		left join [GroupUsers] gu on gu.[GroupId] = gr.[GroupId]
	where (g.[AudienceId]=@InternalAudienceId or g.[AudienceId]=1 or g.[AudienceId]=0) and gu.[ApiUserId]=@ApiUserId

	
	insert into @RoleIds
	select r.[Id]
	from [Roles] r 
		left join [ApiUserRoles] acr on r.[Id] = acr.[RoleId]
	where acr.[ApiUserId] = @ApiUserId


	select r.[Id], r.[Name], r.[Title], r.[IsHidden], r.[Type],
		r.[CreatedDate], r.[CreatedByUserId], r.[CreatedByClientId], 
		r.[UpdatedByUserId], r.[UpdatedByClientId], r.[UpdatedDate], r.[AudienceId]
	from [Roles] r
	where r.[Id] in (
		select distinct [Id]
		FROM @RoleIds
	)

