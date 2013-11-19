-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE PageRecords
 @ownerId int,
 @pageSize int,
 @offset int
AS
 select * from Record
 where ownerId = @ownerId order by transactionDate desc offset @offset rows fetch next @pageSize rows only;
GO