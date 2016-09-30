CREATE PROCEDURE [auth].[auth_ApiClients_Add]
	@ClientId varchar(32),
	@ClientSecretHash varchar(max),
	@ClientSecretSalt varchar(max),
	@Type int,
	@TokenExpireTimeMinutes int,
	@CreatedByUsername varchar(2048) = null,
	@CreatedByClientId varchar(32) = null
AS
begin
	declare @CreatedByUserId int
	declare @CreatedByClientIId int
	declare @InternalId int

	if @CreatedByUsername is not null 
		select @CreatedByUserId=[Id] from [ApiUsers] where [Username]=@CreatedByUsername

	if @CreatedByClientId is not null
		select @CreatedByClientIId=[Id] from [ApiClients] where [ClientId]=@CreatedByClientId

	insert into [ApiClients]([ClientId], [ClientSecretHash], [ClientSecretSalt], [Type], [TokenExpireTimeMinutes], [CreatedByUserId], [CreatedByClientId], [UpdatedByUserId], [UpdatedByClientId], [UpdatedDate])
	values (@ClientId, @ClientSecretHash, @ClientSecretSalt, @Type, @TokenExpireTimeMinutes, @CreatedByUserId, @CreatedByClientIId, @CreatedByUserId, @CreatedByClientIId, getdate())
	
	select @InternalId=SCOPE_IDENTITY()

	select [SystemId] from [ApiClients] where [Id]=@InternalId
end