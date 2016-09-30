CREATE PROCEDURE [auth].[auth_Groups_Add]
	@Name varchar(2048),
	@Description varchar(4096),
	@AudienceId varchar(32),
	@CreatedByUsername varchar(2048) = null,
	@CreatedByClientId varchar(32) = null,
	@SystemId bit
AS
begin
	declare @CreatedByUserId int
	declare @CreatedByClientIId int

	if @CreatedByUsername is not null 
		select @CreatedByUserId=[Id] from [ApiUsers] where [Username]=@CreatedByUsername

	if @CreatedByClientId is not null
		select @CreatedByClientIId=[Id] from [ApiClients] where [ClientId]=@CreatedByClientId

	insert into [Groups] ([Name],[Description], [AudienceId], [CreatedByUserId],[CreatedByClientId], [UpdatedByUserId], [UpdatedByClientId],[UpdatedDate],[IsSystemId])
	values (@Name,@Description, @AudienceId, @CreatedByUserId,@CreatedByClientIId, @CreatedByUserId,@CreatedByClientIId, getdate(),@SystemId )
	
	select SCOPE_IDENTITY()
end
