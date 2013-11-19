-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetRecordsByAccount
 @ownerId int,
 @accountId int
AS
 select * from Record
 where ownerId = @ownerId and accountId = @accountId order by transactionDate desc;
GO