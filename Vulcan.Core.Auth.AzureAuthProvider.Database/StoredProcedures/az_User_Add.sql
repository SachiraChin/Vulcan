CREATE PROCEDURE [aad].[az_User_Add]
	@oid varchar(36),
	@tid varchar(36),
	@upn varchar(512),
	@unique_name varchar(512),
	@family_name varchar(256),
	@given_name varchar(256)
AS
begin
	declare @UserId int
	insert into [Users] ([oid], [tid], [upn], [unique_name], [family_name], [given_name])
	values (@oid, @tid, @upn, @unique_name, @family_name, @given_name)

	select @UserId=SCOPE_IDENTITY()
	
	select @UserId
end