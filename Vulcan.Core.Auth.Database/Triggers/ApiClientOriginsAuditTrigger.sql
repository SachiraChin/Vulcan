GO
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE [type] = 'TR' AND [name] = '[$(NewSchema)].[ApiClientOriginsAuditTrigger]'
    )
DROP TRIGGER [$(NewSchema)].[ApiClientOriginsAuditTrigger];

GO

CREATE TRIGGER [$(NewSchema)].[ApiClientOriginsAuditTrigger]
	ON [$(NewSchema)].[ApiClientOrigins]
	after DELETE, INSERT, UPDATE
	AS
	BEGIN
		declare @TableName varchar(64)
		set @TableName = '[auth].[ApiClientOrigins]'

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
							CAST([ApiClientId] AS nVARCHAR(max)) AS [ApiClientId],
							CAST([Origin] AS nVARCHAR(max)) AS [Origin],
							CAST([CreatedByUserId] AS nVARCHAR(max)) AS [CreatedByUserId],
							CAST([CreatedByClientId] AS nVARCHAR(max)) AS [CreatedByClientId],
							CAST([CreatedDate] AS nVARCHAR(max)) AS [CreatedDate],
							[UpdatedByUserId],
							[UpdatedByClientId],
							CAST([UpdatedDate] AS nVARCHAR(max)) AS [UpdatedDate],
							CAST([IsDeleted] AS nVARCHAR(max)) AS [IsDeleted]
							from inserted
						) ii
						UNPIVOT
						(
							FieldValue
							FOR [FieldName] IN ([ApiClientId],[Origin],[CreatedByUserId],[CreatedByClientId],[CreatedDate],
								[UpdatedDate],[IsDeleted])
						) AS P1
						
					) i
					INNER JOIN (
						select [Id], [FieldName], [FieldValue], [UpdatedByUserId], [UpdatedByClientId]
						from (
							select  [Id], 
							CAST([ApiClientId] AS nVARCHAR(max)) AS [ApiClientId],
							CAST([Origin] AS nVARCHAR(max)) AS [Origin],
							CAST([CreatedByUserId] AS nVARCHAR(max)) AS [CreatedByUserId],
							CAST([CreatedByClientId] AS nVARCHAR(max)) AS [CreatedByClientId],
							CAST([CreatedDate] AS nVARCHAR(max)) AS [CreatedDate],
							[UpdatedByUserId],
							[UpdatedByClientId],
							CAST([UpdatedDate] AS nVARCHAR(max)) AS [UpdatedDate],
							CAST([IsDeleted] AS nVARCHAR(max)) AS [IsDeleted]
							from deleted
						) dd
						UNPIVOT
						(
							FieldValue
							FOR [FieldName] IN ([ApiClientId],[Origin],[CreatedByUserId],[CreatedByClientId],[CreatedDate],
								[UpdatedDate],[IsDeleted])
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
							CAST([ApiClientId] AS nVARCHAR(max)) AS [ApiClientId],
							CAST([Origin] AS nVARCHAR(max)) AS [Origin],
							CAST([CreatedByUserId] AS nVARCHAR(max)) AS [CreatedByUserId],
							CAST([CreatedByClientId] AS nVARCHAR(max)) AS [CreatedByClientId],
							CAST([CreatedDate] AS nVARCHAR(max)) AS [CreatedDate],
							[UpdatedByUserId],
							[UpdatedByClientId],
							CAST([UpdatedDate] AS nVARCHAR(max)) AS [UpdatedDate],
							CAST([IsDeleted] AS nVARCHAR(max)) AS [IsDeleted]
							from inserted
						) ii
						UNPIVOT
						(
							FieldValue
							FOR [FieldName] IN ([ApiClientId],[Origin],[CreatedByUserId],[CreatedByClientId],[CreatedDate],
								[UpdatedDate],[IsDeleted])
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
							CAST([ApiClientId] AS nVARCHAR(max)) AS [ApiClientId],
							CAST([Origin] AS nVARCHAR(max)) AS [Origin],
							CAST([CreatedByUserId] AS nVARCHAR(max)) AS [CreatedByUserId],
							CAST([CreatedByClientId] AS nVARCHAR(max)) AS [CreatedByClientId],
							CAST([CreatedDate] AS nVARCHAR(max)) AS [CreatedDate],
							[UpdatedByUserId],
							[UpdatedByClientId],
							CAST([UpdatedDate] AS nVARCHAR(max)) AS [UpdatedDate],
							CAST([IsDeleted] AS nVARCHAR(max)) AS [IsDeleted]
							from deleted
						) ii
						UNPIVOT
						(
							FieldValue
							FOR [FieldName] IN ([ApiClientId],[Origin],[CreatedByUserId],[CreatedByClientId],[CreatedDate],
								[UpdatedDate],[IsDeleted])
						) A
				) t

		END
	END
	

GO