CREATE PROCEDURE [core].[base_FieldValidation_Add]
	@ValidatorId uniqueidentifier,
	@Message varchar(max),
	@Data varchar(max),
	@FieldId int
AS
BEGIN
	
	declare @FieldName varchar(64)

	select top 1 @FieldName=[Name]
	from [Fields]
	where [Id] = @FieldId

	insert into [FieldValidations]([FieldId], [ValidatorId], [Message], [Data],[FieldName]) 
	values (@FieldId, @ValidatorId, @Message, @Data, @FieldName)

	SELECT SCOPE_IDENTITY()
END
