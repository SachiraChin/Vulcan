CREATE PROCEDURE [auth].[auth_Organizations_Update]
	@Id int,
	@Name varchar(2048),
	@Address varchar(4096),
	@Email varchar(50),
	@Timezone varchar(50),
	@UpdatedByUsername varchar(32) = null,
	@UpdatedByClientId varchar(32) = null
AS
begin
	declare @UpdatedByUserId int
	declare @UpdatedByClientIId int
	
	if @UpdatedByUsername is not null
		select @UpdatedByUserId=[Id] from [ApiUsers] where [Username]=@UpdatedByUsername

    if @UpdatedByClientId is not null
		select @UpdatedByClientIId=[Id] from [ApiClients] where [ClientId]=@UpdatedByClientId

	update [Organizations] set [Name]=@Name,[Address]=@Address,[Email]=@Email,[Timezone]=@Timezone,[UpdatedByClientId]=@UpdatedByClientIId,[UpdatedByUserId]=@UpdatedByUserId,[UpdatedDate]=getdate()
	where [Id]=@Id
	end