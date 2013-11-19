-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetRecordLines
 @recordId int,
 @businessId int
AS
 select * from RecordLine, Inventory
 where RecordLine.recordId = @recordId and Inventory.upc = RecordLine.upc and Inventory.businessId = @businessId;
GO