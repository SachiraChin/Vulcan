CREATE PROCEDURE [auth].[auth_Groups_Update]
	@Id int,
	@Name varchar(2048),
	@Description varchar(4096),
	@AudienceId varchar(32),
	@UpdatedByUsername varchar(2048) = null,
	@UpdatedByClientId varchar(32) = null,
	@SystemId bit

AS
	declare @UpdatedByUserId int
	declare @UpdatedByClientIId int
begin
	if @UpdatedByUsername is not null 
		select @UpdatedByUserId=[Id] from [ApiUsers] where [Username]=@UpdatedByUsername

	if @UpdatedByClientId is not null
		select @UpdatedByClientIId=[Id] from [ApiClients] where [ClientId]=@UpdatedByClientId

	update [Groups] 
	set [Name]=@Name, [Description]=@Description, [AudienceId]=@AudienceId, [UpdatedByUserId]=@UpdatedByUserId,[UpdatedByClientId]=@UpdatedByClientIId,[UpdatedDate]=getdate(),[IsSystemId]=@SystemId
	where [Id] = @Id
end