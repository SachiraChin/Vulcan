CREATE PROCEDURE [auth].[auth_Audiences_GetByAudienceId]
	@audinceId varchar(64)
AS
	select [Id], [AudienceId], [Secret], [Name], 
		[CreatedDate], [CreatedByUserId], [CreatedByClientId], 
		[UpdatedByUserId], [UpdatedByClientId], [UpdatedDate]
	from [Audiences]
	where [AudienceId] = @audinceId
