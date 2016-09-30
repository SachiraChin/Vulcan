CREATE PROCEDURE [auth].[auth_ApiClientOrigins_GetByClientId]
	@clientId varchar(32)
AS
	select aco.[Id], aco.[ApiClientId], aco.[Origin], 
		aco.[CreatedDate], aco.[CreatedByUserId], aco.[CreatedByClientId], 
		aco.[UpdatedByUserId], aco.[UpdatedByClientId], aco.[UpdatedDate]
	from [ApiClientOrigins] aco, [ApiClients] ac
	where aco.[IsDeleted]=0 and aco.[ApiClientId] = ac.[Id] and ac.[ClientId]=@clientId