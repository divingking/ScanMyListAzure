-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE CreateRecord
	-- Add the parameters for the stored procedure here
	@category int,
	@accountId int,
	@ownerId int,
	@clientId int,
	@status int,
	@title varchar(50),
	@comment varchar(140),
	@transactionDate datetimeoffset(7),
	@deliveryDate datetimeoffset(7)
AS
	insert into Record values(@category, @accountId, @ownerId, @clientId, @status, @title,@comment, @transactionDate, @deliveryDate);
GO