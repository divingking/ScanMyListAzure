-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE CreateProduct
	-- Add the parameters for the stored procedure here
	@upc varchar(20)
AS
if not exists (select * from Product where upc = @upc)
begin
	insert into Product values(@upc);
end
GO