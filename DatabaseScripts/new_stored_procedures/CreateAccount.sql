-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	count number of records
-- =============================================
CREATE PROCEDURE CreateAccount
	-- Add the parameters for the stored procedure here
	@businessId int,
	@login varchar(256),
	@password varchar(128),
	@tier int,
	@firstName varchar(50),
	@lastName varchar(50),
	@email varchar(200),
	@phoneNumber varchar(20),
	@sessionId varchar(512),
	@deviceId varchar(128)
AS
if not exists (select * from Account where login = @login)
begin
	insert into Account values(@businessId, @login, @password, @tier, @firstName, @lastName, @email, @phoneNumber, @sessionId, @deviceId);
	return (select ident_current('Account'));
end
else
	return -1;
GO