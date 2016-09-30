CREATE PROCEDURE [core].[base_Migration_Add]
	@id uniqueidentifier
AS
	INSERT INTO [Migrations]([Id]) VALUES(@id)