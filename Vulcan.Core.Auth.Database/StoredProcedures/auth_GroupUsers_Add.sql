CREATE PROCEDURE [auth].[auth_GroupUsers_Add]
	@GroupId int,
	@Ids [BigIntList] READONLY,
	@CreatedByUsername varchar(2048) = null,
	@CreatedByClientId varchar(32) = null
AS
begin
	declare @CreatedByUserId int
	declare @CreatedByClientIId int
	declare @createdDate datetime
	declare @InternalUserIds [IntList]
		select @createdDate=getdate()

	if @CreatedByUsername is not null 
		select @CreatedByUserId=[Id] from [ApiUsers] where [Username]=@CreatedByUsername

	if @CreatedByClientId is not null
		select @CreatedByClientIId=[Id] from [ApiClients] where [ClientId]=@CreatedByClientId
	
	insert into @InternalUserIds([Value])
	select [Id]
	from [ApiUsers]
	where [SystemId] in (
		select [Value]
		from @Ids
	)

	insert into [GroupUsers]([GroupId],[ApiUserId],[CreatedByUserId], [CreatedByClientId], [UpdatedByUserId], [UpdatedByClientId], [UpdatedDate]) 
	select @GroupId,[Value],@CreatedByUserId,@CreatedByClientIId,@CreatedByUserId,@CreatedByClientIId,@createdDate
	from @InternalUserIds
end
