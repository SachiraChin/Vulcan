﻿GO
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE [type] = 'TR' AND [name] = '[$(NewSchema)].[AudiencesAuditTrigger]'
    )
DROP TRIGGER [$(NewSchema)].[AudiencesAuditTrigger];

GO

CREATE TRIGGER [$(NewSchema)].[AudiencesAuditTrigger]
	ON [$(NewSchema)].[Audiences]
	FOR DELETE, INSERT, UPDATE
	AS
	BEGIN
		declare @TableName varchar(64)
		set @TableName = '[auth].[Audiences]'

		IF EXISTS (SELECT * FROM INSERTED)
		BEGIN
			IF EXISTS (SELECT * FROM DELETED)
			BEGIN
				--UPDATE
				INSERT INTO [$(NewSchema)].[Audits]([Type], [TableName], [PrimaryKeyField],[PrimaryKeyValue],[FieldName],[OldValue],[NewValue],[CreatedByUserId],[CreatedByClientId])
				SELECT 'U', @TableName, '[Id]', [Id], [FieldName], [OldValue], [NewValue], [UpdatedByUserId], [UpdatedByClientId]
				FROM (
					SELECT i.Id, i.[FieldName], i.[FieldValue] as [NewValue], d.[FieldValue] as [OldValue], i.[UpdatedByUserId], i.[UpdatedByClientId]
					FROM 
					(
						select [Id], [FieldName], [FieldValue], [UpdatedByUserId], [UpdatedByClientId]
						from (
							select  [Id], 
							CAST([AudienceId] AS nVARCHAR(max)) AS [AudienceId],
							CAST([Secret] AS nVARCHAR(max)) AS [Secret],
							CAST([Name] AS nVARCHAR(max)) AS [Name],
							CAST([CreatedByUserId] AS nVARCHAR(max)) AS [CreatedByUserId],
							CAST([CreatedByClientId] AS nVARCHAR(max)) AS [CreatedByClientId],
							CAST([CreatedDate] AS nVARCHAR(max)) AS [CreatedDate],
							[UpdatedByUserId],
							[UpdatedByClientId],
							CAST([UpdatedDate] AS nVARCHAR(max)) AS [UpdatedDate]
							from inserted
						) ii
						UNPIVOT
						(
							FieldValue
							FOR [FieldName] IN (
								[AudienceId],[Secret],[Name],
								[CreatedByUserId],[CreatedByClientId],[CreatedDate], [UpdatedDate]
							)
						) AS P1
						
					) i
					INNER JOIN (
						select [Id], [FieldName], [FieldValue], [UpdatedByUserId], [UpdatedByClientId]
						from (
							select  [Id], 
							CAST([AudienceId] AS nVARCHAR(max)) AS [AudienceId],
							CAST([Secret] AS nVARCHAR(max)) AS [Secret],
							CAST([Name] AS nVARCHAR(max)) AS [Name],
							CAST([CreatedByUserId] AS nVARCHAR(max)) AS [CreatedByUserId],
							CAST([CreatedByClientId] AS nVARCHAR(max)) AS [CreatedByClientId],
							CAST([CreatedDate] AS nVARCHAR(max)) AS [CreatedDate],
							[UpdatedByUserId],
							[UpdatedByClientId],
							CAST([UpdatedDate] AS nVARCHAR(max)) AS [UpdatedDate]
							from deleted
						) dd
						UNPIVOT
						(
							FieldValue
							FOR [FieldName] IN (
								[AudienceId],[Secret],[Name],
								[CreatedByUserId],[CreatedByClientId],[CreatedDate], [UpdatedDate]
							)
						) AS P2
						
					) d ON i.ID = d.ID and i.[FieldName]=d.[FieldName]
					where i.[FieldValue] <> d.[FieldValue]
				) t
			END
			ELSE
			BEGIN
				--INSERT
				INSERT INTO [$(NewSchema)].[Audits]([Type], [TableName], [PrimaryKeyField],[PrimaryKeyValue],[FieldName],[OldValue],[NewValue],[CreatedByUserId],[CreatedByClientId])
				SELECT 'I', @TableName, '[Id]', [Id], [FieldName], null, [FieldValue], [UpdatedByUserId], [UpdatedByClientId]
				FROM (
						select [Id], [FieldName], [FieldValue], [UpdatedByUserId], [UpdatedByClientId]
						from (
							select  [Id], 
							CAST([AudienceId] AS nVARCHAR(max)) AS [AudienceId],
							CAST([Secret] AS nVARCHAR(max)) AS [Secret],
							CAST([Name] AS nVARCHAR(max)) AS [Name],
							CAST([CreatedByUserId] AS nVARCHAR(max)) AS [CreatedByUserId],
							CAST([CreatedByClientId] AS nVARCHAR(max)) AS [CreatedByClientId],
							CAST([CreatedDate] AS nVARCHAR(max)) AS [CreatedDate],
							[UpdatedByUserId],
							[UpdatedByClientId],
							CAST([UpdatedDate] AS nVARCHAR(max)) AS [UpdatedDate]
							from inserted
						) ii
						UNPIVOT
						(
							FieldValue
							FOR [FieldName] IN (
								[AudienceId],[Secret],[Name],
								[CreatedByUserId],[CreatedByClientId],[CreatedDate], [UpdatedDate]
							)
						) A
				) t
			END

		END
		ELSE IF EXISTS(SELECT * FROM DELETED)
		BEGIN
			--DELETE
				INSERT INTO [$(NewSchema)].[Audits]([Type], [TableName], [PrimaryKeyField],[PrimaryKeyValue],[FieldName],[OldValue],[NewValue],[CreatedByUserId],[CreatedByClientId])
				SELECT 'D', @TableName, '[Id]', [Id], [FieldName], [FieldValue], null, [UpdatedByUserId], [UpdatedByClientId]
				FROM (
						select [Id], [FieldName], [FieldValue], [UpdatedByUserId], [UpdatedByClientId]
						from (
							select  [Id], 
							CAST([AudienceId] AS nVARCHAR(max)) AS [AudienceId],
							CAST([Secret] AS nVARCHAR(max)) AS [Secret],
							CAST([Name] AS nVARCHAR(max)) AS [Name],
							CAST([CreatedByUserId] AS nVARCHAR(max)) AS [CreatedByUserId],
							CAST([CreatedByClientId] AS nVARCHAR(max)) AS [CreatedByClientId],
							CAST([CreatedDate] AS nVARCHAR(max)) AS [CreatedDate],
							[UpdatedByUserId],
							[UpdatedByClientId],
							CAST([UpdatedDate] AS nVARCHAR(max)) AS [UpdatedDate]
							from deleted
						) ii
						UNPIVOT
						(
							FieldValue
							FOR [FieldName] IN (
								[AudienceId],[Secret],[Name],
								[CreatedByUserId],[CreatedByClientId],[CreatedDate],[UpdatedDate]
							)
						) A
				) t

		END
	END

GO