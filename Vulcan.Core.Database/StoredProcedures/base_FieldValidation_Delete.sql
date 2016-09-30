CREATE PROCEDURE [core].[base_FieldValidation_Delete]
	@id int
AS
	UPDATE [FieldValidations] SET IsDeleted=1 WHERE Id=@id
	RETURN @id
