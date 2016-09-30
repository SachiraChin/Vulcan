CREATE PROCEDURE [core].[base_MigrationEntry_Add]
	@MigrationId uniqueidentifier,
	@TableName varchar(64),
	@EntryJson nvarchar(max),
	@ExecutionOrderIndex int
AS
	insert into [MigrationEntries]([MigrationId], [TableName],[EntryJson], [ExecutionOrderIndex])
	values (@MigrationId, @TableName, @EntryJson, @ExecutionOrderIndex)