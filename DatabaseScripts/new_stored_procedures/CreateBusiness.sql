-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE CreateBusiness
	-- Add the parameters for the stored procedure here
	@name varchar(100),
	@integration int,
	@tier int,
	@address varchar(200),
	@postalCode varchar(10),
	@email varchar(200),
	@phoneNumber varchar(20)
AS
if not exists (select * from Business where name = @name and postalCode = @postalCode)
begin
	insert into Business values(@name, @integration, @tier, @address, @postalCode, @email, @phoneNumber);
	return (select ident_current('Business'));
end
else
	return -1;
GO