﻿CREATE PROCEDURE [core].[base_Field_GetById]
	@id int
AS
	SELECT [Id]
      ,[TableName]
      ,[Name]
      ,[Title]
      ,[Type]
      ,[IsHidden]
      ,[IsDeleted]
      ,[IsSystem]
      -- ,[InternalName]
	  ,[IsAutoGenerated],[ShowInUi],[UiIndex]
  FROM [Fields]
  WHERE [Id]=@id and [IsDeleted]=0 and [IsHidden]=0
