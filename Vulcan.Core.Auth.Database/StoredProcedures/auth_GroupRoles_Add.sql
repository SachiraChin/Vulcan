CREATE PROCEDURE [auth].[auth_GroupRoles_Add]
	@GroupId int,
	@Ids [IntList] READONLY,
	@CreatedByUsername varchar(2048) = null,
	@CreatedByClientId varchar(32) = null
AS
begin
	declare @CreatedByUserId int
	declare @CreatedByClientIId int
	declare @createdDate datetime

	select @createdDate=getdate()

	if @CreatedByUsername is not null 
		select @CreatedByUserId=[Id] from [ApiUsers] where [Username]=@CreatedByUsername

	if @CreatedByClientId is not null
		select @CreatedByClientIId=[Id] from [ApiClients] where [ClientId]=@CreatedByClientId

	insert into [GroupRoles]([GroupId],[RoleId],[CreatedByUserId], [CreatedByClientId], [UpdatedByUserId], [UpdatedByClientId], [UpdatedDate]) 
	select @GroupId, [Value],@CreatedByUserId,@CreatedByClientIId,@CreatedByUserId,@CreatedByClientIId,@createdDate
	from @Ids
end
