CREATE PROCEDURE [auth].[auth_Roles_AddApiUserRoles]
	@userSystemId bigint = 0,
	@Ids [IntList] READONLY,
	@CreatedByUsername varchar(2048) = null,
	@CreatedByClientId varchar(32) = null
AS
begin
	declare @id int
	declare @CreatedByUserId int
	declare @CreatedByClientIId int
	declare @createdDate datetime

	select @createdDate=getdate()

	if @CreatedByUsername is not null 
		select @CreatedByUserId=[Id] from [ApiUsers] where [Username]=@CreatedByUsername

	if @CreatedByClientId is not null
		select @CreatedByClientIId=[Id] from [ApiClients] where [ClientId]=@CreatedByClientId

	select @id=[Id] from [ApiUsers] where [SystemId]=@userSystemId
	
	insert into [ApiUserRoles] ([ApiUserId], [RoleId],[CreatedByUserId], [CreatedByClientId], [UpdatedByUserId], [UpdatedByClientId], [UpdatedDate])
	select @id, [Value], @CreatedByUserId, @CreatedByClientIId, @CreatedByUserId, @CreatedByClientIId, @createdDate
	from @Ids
end