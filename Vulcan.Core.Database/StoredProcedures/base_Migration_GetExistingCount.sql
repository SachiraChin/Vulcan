CREATE PROCEDURE [core].[base_Migration_GetExistingCount]
	@ids [UniqueIdentifierList] READONLY
AS
	SELECT count([Id]) from [Migrations]
	where [Id] in (SELECT [Value] FROM @ids)
