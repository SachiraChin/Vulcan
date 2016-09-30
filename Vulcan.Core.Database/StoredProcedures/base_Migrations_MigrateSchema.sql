CREATE PROCEDURE [core].[base_Migrations_MigrateSchema]
	@SchemaName varchar(64)
AS
BEGIN
	declare @CreateSchemaSql nvarchar(max)
	
	set @CreateSchemaSql = N'CREATE SCHEMA ' + @SchemaName
	EXECUTE sp_executesql @CreateSchemaSql
END