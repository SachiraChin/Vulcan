CREATE PROCEDURE [auth].[auth_Groups_Delete]
	@Id int

AS
	update [Groups] set [Name]= CONCAT('deleted_',[Name]), [IsDeleted]=1 where [Id]=@Id
