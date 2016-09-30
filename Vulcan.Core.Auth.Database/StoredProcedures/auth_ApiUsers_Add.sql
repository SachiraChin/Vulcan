CREATE PROCEDURE [auth].[auth_ApiUsers_Add]
	@Username varchar(2048),
	@PasswordHash varchar(max),
	@PasswordSalt varchar(max),
	@TokenExpireTimeMinutes int = null,
	@CreatedByUsername varchar(2048) = null,
	@CreatedByClientId varchar(32) = null,
	@Type int = null,
	@ExternalRefId int = null,
	@ExternalProviderName varchar(256) = null,
	@FirstName nvarchar(256) = null,
	@LastName nvarchar(256) = null,
	@DisplayName nvarchar(512) = null
AS
begin
	
	declare @CreatedByUserId int
	declare @CreatedByClientIId int
	declare @InternalId int
	if @CreatedByUsername is not null 
		select @CreatedByUserId=[Id] from [ApiUsers] where [Username]=@CreatedByUsername

	if @CreatedByClientId is not null
		select @CreatedByClientIId=[Id] from [ApiClients] where [ClientId]=@CreatedByClientId

	insert into [ApiUsers] ([Username], [PasswordHash], [PasswordSalt], [TokenExpireTimeMinutes], [CreatedByUserId], [CreatedByClientId], [UpdatedByUserId], [UpdatedByClientId], [UpdatedDate], [Type], [ExternalRefId], [ExternalProviderName], [FirstName], [LastName], [DisplayName])
	values (@Username, @PasswordHash, @PasswordSalt, @TokenExpireTimeMinutes, @CreatedByUserId, @CreatedByClientIId, @CreatedByUserId, @CreatedByClientIId, getdate(), @Type, @ExternalRefId,@ExternalProviderName,@FirstName, @LastName, @DisplayName)
	
	select @InternalId=SCOPE_IDENTITY()

	select [SystemId] from [ApiUsers] where [Id]=@InternalId
end