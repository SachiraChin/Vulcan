CREATE PROCEDURE [auth].[auth_ApiUsers_Update]
	@Id bigint,
	@Username varchar(2048),
	@PasswordHash varchar(max),
	@PasswordSalt varchar(max),
	@TokenExpireTimeMinutes int = null,
	@UpdatedByUsername varchar(2048) = null,
	@UpdatedByClientId varchar(32) = null,
	@FirstName nvarchar(256) = null,
	@LastName nvarchar(256) = null,
	@DisplayName nvarchar(512) = null
AS
begin
	declare @UpdatedByUserId int
	declare @UpdatedByClientIId int

	if @UpdatedByUsername is not null 
		select @UpdatedByUserId=[Id] from [ApiUsers] where [Username]=@UpdatedByUsername

	if @UpdatedByClientId is not null
		select @UpdatedByClientIId=[Id] from [ApiClients] where [ClientId]=@UpdatedByClientId

	update [ApiUsers] set [Username]=@Username, [PasswordHash]=@PasswordHash, [PasswordSalt]=@PasswordSalt,[TokenExpireTimeMinutes]=@TokenExpireTimeMinutes,
			[UpdatedByClientId]=@UpdatedByClientIId, [UpdatedByUserId]=@UpdatedByUserId, [UpdatedDate]=getdate(), [FirstName]=@FirstName, [LastName]=@LastName, [DisplayName]=@DisplayName
	where [SystemId] = @Id
end