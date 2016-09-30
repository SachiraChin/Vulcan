
GO

--- Predefined audiences
SET IDENTITY_INSERT [$(NewSchema)].[Audiences] ON
---		Add auth service audience
insert into [$(NewSchema)].[Audiences]([Id], [AudienceId], [Secret], [Name]) 
values(1, 'VulcanAuthApi', '5BdJQYkOipboj3FiAgpJbFPov72ieFx1U3-dq7HSsUw', 'Core Service')

SET IDENTITY_INSERT [$(NewSchema)].[Audiences] OFF

GO


--- Predefined users
SET IDENTITY_INSERT [$(NewSchema)].[ApiUsers] ON

---		Add super admin
insert into [$(NewSchema)].[ApiUsers] ([Id], [Username], [PasswordHash], [PasswordSalt], [UpdatedDate],[IsSystem]) 
values (1, 'admin', 'MAjVQ2AG/NquXFtfNYx2f/Q11ZBwRciL', '7xD+pZFnklBbjqtko5jBYRDgq5Au1lgV', getdate(),1)-- Lj&e#N8

---		Add all users account

insert into [$(NewSchema)].[ApiUsers] ([Id], [Username], [CreatedByUserId], [UpdatedByUserId], [UpdatedDate]) 
values (0, 'NETWORK\ALL USERS',1,1, getdate())

SET IDENTITY_INSERT [$(NewSchema)].[ApiUsers] OFF

GO


----- Predefined api clients
--SET IDENTITY_INSERT [$(NewSchema)].[ApiClients] ON

-----		Add api client for auth service access
--insert into [$(NewSchema)].[ApiClients] ([Id], [ClientId], [ClientSecretHash], [ClientSecretSalt], [Type], [CreatedByUserId], [UpdatedByUserId], [UpdatedDate]) 
--values (1, '382649d317564ee08f27f20d3ef55d42', 'f+z1F9ytY88GkdmITZD4P4YK1OgMnjPv', 'wr1bAmKccA+JaDIcyys+sYuWhVDry1So', 1, 1, 1, getdate()) -- iTK16YxqUVQFPdVm0iuG0PtCAPuhLv06-_ZDuCSuKI0

--SET IDENTITY_INSERT [$(NewSchema)].[ApiClients] OFF




--- Predefined roles
SET IDENTITY_INSERT [$(NewSchema)].[Roles] ON

---		Add super admin role
insert into [$(NewSchema)].[Roles]([Id], [Name], [Title], [Type], [IsHidden], [CreatedByUserId], [UpdatedByUserId], [UpdatedDate],[AudienceId])
values (1, 'sa', 'Super Admin', 99, 1, 1, 1, getdate(),1)
---		Add system roles
insert into [$(NewSchema)].[Roles]([Id], [Name], [Title], [Type], [IsHidden], [CreatedByUserId], [UpdatedByUserId], [UpdatedDate],[AudienceId])
values (2, 'add', 'Add', 1, 0, 1, 1, getdate(),1)
insert into [$(NewSchema)].[Roles]([Id], [Name], [Title], [Type], [IsHidden], [CreatedByUserId], [UpdatedByUserId], [UpdatedDate],[AudienceId])
values (3, 'update', 'Update', 1, 0, 1, 1, getdate(),1)
insert into [$(NewSchema)].[Roles]([Id], [Name], [Title], [Type], [IsHidden], [CreatedByUserId], [UpdatedByUserId], [UpdatedDate],[AudienceId])
values (4, 'delete', 'Delete', 1, 0, 1, 1, getdate(),1)
insert into [$(NewSchema)].[Roles]([Id], [Name], [Title], [Type], [IsHidden], [CreatedByUserId], [UpdatedByUserId], [UpdatedDate],[AudienceId])
values (5, 'get', 'Get', 1, 0, 1, 1, getdate(),1)
insert into [$(NewSchema)].[Roles]([Id], [Name], [Title], [Type], [IsHidden], [CreatedByUserId], [UpdatedByUserId], [UpdatedDate],[AudienceId])
values (6, 'approve', 'Approve', 1, 0, 1, 1, getdate(),1)

SET IDENTITY_INSERT [$(NewSchema)].[Roles] OFF


GO


--- Predefined user roles
SET IDENTITY_INSERT [$(NewSchema)].[ApiUserRoles] ON

---		Assign super admin role to super admin
insert into [$(NewSchema)].[ApiUserRoles]([Id], [ApiUserId], [RoleId], [CreatedByUserId], [UpdatedByUserId], [UpdatedDate]) 
values (1, 1, 1, 1, 1,getdate())

SET IDENTITY_INSERT [$(NewSchema)].[ApiUserRoles] OFF

GO



----- Predefined user roles
--SET IDENTITY_INSERT [$(NewSchema)].[ApiClientRoles] ON

-----		Assign super admin role to super admin
--insert into [$(NewSchema)].[ApiClientRoles]([Id], [ApiClientId], [RoleId], [CreatedByUserId], [UpdatedByUserId], [UpdatedDate]) 
--values (1, 1, 2, 1, 1,getdate())

--SET IDENTITY_INSERT [$(NewSchema)].[ApiClientRoles] OFF

