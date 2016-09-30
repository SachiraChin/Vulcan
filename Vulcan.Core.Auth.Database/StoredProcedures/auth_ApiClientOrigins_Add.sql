CREATE PROCEDURE [auth].[auth_ApiClientOrigins_Add]
	@ClientId int,
	@Origin nvarchar(2048),
	@CreatedByUsername varchar(2048) = null,
	@CreatedByClientId varchar(32) = null
AS
begin
	--declare @clientIId int
	declare @CreatedByUserId int
	declare @CreatedByClientIId int

	if @CreatedByUsername is not null 
		select @CreatedByUserId=[Id] from [ApiUsers] where [Username]=@CreatedByUsername

	if @CreatedByClientId is not null
		select @CreatedByClientIId=[Id] from [ApiClients] where [ClientId]=@CreatedByClientId

	--select @clientIId=[Id] from [ApiClients] where [ClientId]=@clientIId

	insert into [ApiClientOrigins] ([Origin], [ApiClientId], [CreatedByUserId], [CreatedByClientId], [UpdatedByUserId], [UpdatedByClientId], [UpdatedDate]) 
	values (@Origin, @ClientId, @CreatedByUserId, @CreatedByClientIId, @CreatedByUserId, @CreatedByClientIId, getdate())

	select SCOPE_IDENTITY()
end
