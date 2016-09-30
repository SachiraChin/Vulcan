CREATE PROCEDURE [core].[base_FieldValidation_Update]
	@Id int,
	@ValidatorId uniqueidentifier,
	@Message varchar(max),
	@Data varchar(max) 
AS
BEGIN
	UPDATE [FieldValidations] SET 
		[ValidatorId]=@ValidatorId,
		[Message]=@Message,
		[Data]=@Data
	WHERE [Id]=@Id

	SELECT @Id
END
