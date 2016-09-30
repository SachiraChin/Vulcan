CREATE PROCEDURE [aad].[az_User_Update]
	@UserId int,
	@oid varchar(36),
	@tid varchar(36),
	@upn varchar(512),
	@unique_name varchar(512),
	@family_name varchar(256),
	@given_name varchar(256)
AS
	update [Users] set [oid]=@oid, [tid]=@tid, [upn]=@upn, [unique_name]=@unique_name, [family_name]=@family_name, [given_name]=@given_name
	where [Id]=@UserId