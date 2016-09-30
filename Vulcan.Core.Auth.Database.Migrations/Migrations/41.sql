IF $(CurrentVersion) > 41
begin
		EXECUTE sp_executesql 
		N'CREATE PROCEDURE [$(NewSchema)].[auth_Audiences_GetAll]
			AS
			select [Id], [AudienceId], [Secret], [Name], [CreatedDate], [CreatedByUserId], [CreatedByClientId], [UpdatedByUserId], [UpdatedByClientId], [UpdatedDate]
			from [Audiences]'
end
