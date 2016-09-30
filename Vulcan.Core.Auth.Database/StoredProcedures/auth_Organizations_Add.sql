CREATE PROCEDURE [auth].[auth_Organizations_Add]
	@Name varchar(2048),
	@Address varchar(4096),
	@Email varchar(50),
	@Timezone varchar(50),
	@CreatedByUsername varchar(2048)=null,
	@CreatedByClientId varchar(32)=null
	
AS
begin
	declare @CreatedByUserId int
	declare @CreatedByClientIId int
	if @CreatedByUsername is not null
		select @CreatedByUserId=[Id] from [ApiUsers] where [Username]=@CreatedByUsername

	if @CreatedByClientId is not null
		select @CreatedByClientIId=[Id] from [ApiClients] where [ClientId]=@CreatedByClientId

	insert into [Organizations]([Name],[Address],[Email],[Timezone],[CreatedByUserId],[UpdatedByUserId],[CreatedByClientId],[UpdatedByClientId], [UpdatedDate])
	values (@Name,@Address,@Email,@Timezone,@CreatedByUserId,@CreatedByUserId,@CreatedByClientIId,@CreatedByClientIId,getdate())
end