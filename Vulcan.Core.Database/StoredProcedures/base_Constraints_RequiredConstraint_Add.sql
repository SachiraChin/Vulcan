CREATE PROCEDURE [core].[base_Constraints_RequiredConstraint_Add]
	@FieldId int,
	@DefaultValue varchar(64) = null
AS
BEGIN
	
	declare @TableName varchar(64)
	declare @FieldName varchar(64)
	declare @DataType varchar(64)

	select top 1 @DataType=[SqlDataType],@TableName=[TableName],@FieldName=[Name]
	from [Fields]
	where [Id]=@FieldId

	if @DataType is not null
	begin
		declare @Sql nvarchar(max)

		set @Sql = N'alter table ' + @TableName + ' alter column ' + @FieldName + ' ' +  @DataType + ' not null'

		if @DefaultValue is not null
		begin
			set @Sql = @Sql + ' default ''' + @DefaultValue + ''''
		end
		
		EXECUTE sp_executesql @Sql
	end
END