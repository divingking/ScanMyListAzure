-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetRecords
 @ownerId int
AS
 select * from Record
 where ownerId = @ownerId
 order by transactionDate desc;
GO