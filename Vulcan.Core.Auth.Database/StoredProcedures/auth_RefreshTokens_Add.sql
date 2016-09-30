CREATE PROCEDURE [auth].[auth_RefreshTokens_Add]
	@TokenHash varchar(64),
	@Subject varchar(32),
	@IssuedDate datetime,
	@ExpiryDate datetime,
	@Ticket varchar(max)
AS
	--IF EXISTS(select [Id] from [RefreshTokens] where [Subject] = @Subject)
	--	update [RefreshTokens] set [TokenHash]=@TokenHash, [IssuedDate]=@IssuedDate, [ExpiryDate]=@ExpiryDate, [Ticket]=@Ticket  where [Subject] = @Subject
 --   ELSE
        insert into [RefreshTokens]([TokenHash], [Subject], [IssuedDate], [ExpiryDate], [Ticket]) values (@TokenHash, @Subject, @IssuedDate, @ExpiryDate, @Ticket)