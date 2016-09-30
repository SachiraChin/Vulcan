CREATE PROCEDURE [aad].[az_AccessCodes_Deactivate]
	@userId int
AS
	update [AccessCodes] set [IsActive]=0 where [ApiUserId]=@userId