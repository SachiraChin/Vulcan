CREATE PROCEDURE [auth].[auth_GroupRoles_Update]
	@GroupId int,
	@Ids [IntList] READONLY,
	@UpdatedByUsername varchar(2048) = null,
	@UpdatedByClientId varchar(32) = null

AS
	declare @UpdatedByUserId int
	declare @UpdatedByClientIId int
begin
	if @UpdatedByUsername is not null 
		select @UpdatedByUserId=[Id] from [ApiUsers] where [Username]=@UpdatedByUsername

	if @UpdatedByClientId is not null
		select @UpdatedByClientIId=[Id] from [ApiClients] where [ClientId]=@UpdatedByClientId

	--update [GroupRoles] 
	--set [RoleId]=@Ids,[UpdatedByUserId]=@UpdatedByUserId,[UpdatedDate]=getdate(),[UpdatedByClientId]=@UpdatedByClientIId
	--where [GroupId] = @GroupId
end