CREATE PROCEDURE [auth].[auth_ApiClientOrigins_Delete]
	@originId int
AS
	update [ApiClientOrigins] set [IsDeleted]=1 where [Id]=@originId
