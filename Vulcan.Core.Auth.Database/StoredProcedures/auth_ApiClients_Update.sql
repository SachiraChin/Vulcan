CREATE PROCEDURE [auth].[auth_ApiClients_Update]
	@Id bigint,
	@ClientId varchar(32),
	@ResetSecret bit,
	@Type int,
	@TokenExpireTimeMinutes int,
	@ClientSecretHash varchar(max) = null,
	@ClientSecretSalt varchar(max) = null,
	@UpdatedByUsername varchar(2048) = null,
	@UpdatedByClientId varchar(32) = null
AS
begin

	declare @UpdatedByUserId int
	declare @UpdatedByClientIId int

	if @UpdatedByUsername is not null 
		select @UpdatedByUserId=[Id] from [ApiUsers] where [Username]=@UpdatedByUsername

	if @UpdatedByClientId is not null
		select @UpdatedByClientIId=[Id] from [ApiClients] where [ClientId]=@UpdatedByClientId

	if @ResetSecret = 0
		update [ApiClients] set [Type]=@Type, [TokenExpireTimeMinutes]=@TokenExpireTimeMinutes, 
			[UpdatedByClientId]=@UpdatedByClientIId, [UpdatedByUserId]=@UpdatedByUserId, [UpdatedDate]=getdate()
		where [SystemId]=@Id and [IsDeleted]=0
	else
		update [ApiClients] set [Type]=@Type, [TokenExpireTimeMinutes]=@TokenExpireTimeMinutes, [ClientSecretHash]=@ClientSecretHash, [ClientSecretSalt]=@ClientSecretSalt,
			[UpdatedByClientId]=@UpdatedByClientIId, [UpdatedByUserId]=@UpdatedByUserId, [UpdatedDate]=getdate()
		where [SystemId]=@Id and [IsDeleted]=0
end