CREATE PROCEDURE [core].[base_FieldChoice_Get]
AS
	select fv.[Id], [FieldId], [Text]
	from [FieldChoices] fv, [Fields] f
  WHERE fv.[FieldId]=f.[Id] and f.[IsDeleted]=0 and f.[IsHidden]=0 and fv.IsDeleted=0
