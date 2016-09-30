CREATE PROCEDURE [core].[base_FieldValidation_Get]
AS
	SELECT fv.[Id]
      ,[FieldId]
      ,[FieldName]
      ,[ValidatorId]
      ,[Message]
      ,[Data]
  FROM [FieldValidations] fv, [Fields] f
  WHERE fv.[FieldId]=f.[Id] and f.[IsDeleted]=0 and f.[IsHidden]=0 and fv.IsDeleted=0

