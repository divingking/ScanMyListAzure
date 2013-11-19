-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetRecordsByClient
 @ownerId int,
 @clientId int
AS
 select * from Record
 where ownerId = @ownerId and clientId = @clientId
 order by transactionDate desc;
GO