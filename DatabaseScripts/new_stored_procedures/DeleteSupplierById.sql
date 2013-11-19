-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE DeleteSupplierById
	@supplierId int
AS
	delete from Supplier where supplierId = @supplierId;
GO