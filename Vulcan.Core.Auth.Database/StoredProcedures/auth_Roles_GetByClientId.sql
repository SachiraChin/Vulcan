﻿CREATE PROCEDURE [auth].[auth_Roles_GetByClientId]
	@clientId varchar(32)
AS
	select r.[Id], r.[Name], r.[Title], r.[IsHidden], r.[Type],
		r.[CreatedDate], r.[CreatedByUserId], r.[CreatedByClientId], 
		r.[UpdatedByUserId], r.[UpdatedByClientId], r.[UpdatedDate], r.[AudienceId]
	from [Roles] r, [ApiClientRoles] acr, [ApiClients] ac
	where r.[Id] = acr.[RoleId] and acr.[ApiClientId] = ac.[Id] and ac.[ClientId] = @clientId
