CREATE PROCEDURE [auth].[auth_ApiClientOrigins_GetById]
	@id int
AS
	select aco.[Id], aco.[ApiClientId], aco.[Origin], 
		aco.[CreatedDate], aco.[CreatedByUserId], aco.[CreatedByClientId], 
		aco.[UpdatedByUserId], aco.[UpdatedByClientId], aco.[UpdatedDate]
	from [ApiClientOrigins] aco
	where aco.[IsDeleted]=0 and aco.Id=@id