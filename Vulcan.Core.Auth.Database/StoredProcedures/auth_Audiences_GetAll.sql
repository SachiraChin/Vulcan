CREATE PROCEDURE [auth].[auth_Audiences_GetAll]
AS
	select [Id], [AudienceId], [Secret], [Name], 
		[CreatedDate], [CreatedByUserId], [CreatedByClientId], 
		[UpdatedByUserId], [UpdatedByClientId], [UpdatedDate]
	from [Audiences]
