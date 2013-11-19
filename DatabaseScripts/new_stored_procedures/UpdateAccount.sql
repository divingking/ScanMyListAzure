-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	count number of records
-- =============================================
CREATE PROCEDURE UpdateAccount
	-- Add the parameters for the stored procedure here
	@accountId int,
	@businessId int,
	@login varchar(256),
	@tier int,
	@firstName varchar(50),
	@lastName varchar(50),
	@email varchar(200),
	@phoneNumber varchar(20)
AS
update Account set 
	businessId = @businessId,
	login = @login,
	tier = @tier,
	firstName = @firstName,
	lastName = @lastName,
	email = @email,
	phoneNumber = @phoneNumber
where id = @accountId;
GO