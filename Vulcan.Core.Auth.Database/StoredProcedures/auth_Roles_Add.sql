CREATE PROCEDURE [auth].[auth_Roles_Add]
	@Name varchar(64),
	@Title varchar(64),
	@IsHidden bit,
	@Type tinyint,
	@AudienceId int,
	@CreatedByUsername varchar(2048) = null,
	@CreatedByClientId varchar(32) = null
AS
begin
	declare @CreatedByUserId int
	declare @CreatedByClientIId int

	if @CreatedByUsername is not null 
		select @CreatedByUserId=[Id] from [ApiUsers] where [Username]=@CreatedByUsername

	if @CreatedByClientId is not null
		select @CreatedByClientIId=[Id] from [ApiClients] where [ClientId]=@CreatedByClientId

	insert into [Roles] ([Name], [Title], [IsHidden], [Type], [CreatedByUserId], [CreatedByClientId], [UpdatedByUserId], [UpdatedByClientId], [UpdatedDate], [AudienceId])
	values (@Name, @Title, @IsHidden, @Type, @CreatedByUserId, @CreatedByClientIId,@CreatedByUserId, @CreatedByClientIId, getdate(), @AudienceId)
	
	select SCOPE_IDENTITY()
end