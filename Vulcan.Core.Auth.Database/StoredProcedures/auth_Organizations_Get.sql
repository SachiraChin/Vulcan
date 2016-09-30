CREATE PROCEDURE [auth].[auth_Organizations_Get]
	
AS
	select [Id],[Name],[Address],[Email],[Timezone],[CreatedByUserId],[CreatedDate],[UpdatedByUserId],[UpdatedDate]
	from [Organizations]
	