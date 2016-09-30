CREATE PROCEDURE [core].[base_Field_Delete]
	@id int
AS
	UPDATE [Fields] SET IsDeleted=1 WHERE Id=@id
	RETURN @id
