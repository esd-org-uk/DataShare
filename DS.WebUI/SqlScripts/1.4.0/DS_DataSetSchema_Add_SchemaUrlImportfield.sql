/*
   10 April 201415:35:20
   User: 
   Server: ICTPC14696\SQLEXPRESS
   Database: DataShare71a
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.DS_DataSetSchema ADD
	SchemaDefinitionFromUrl nvarchar(2048) NULL
GO
COMMIT
/*ALTER TABLE dbo.DS_DataSetSchema SET (LOCK_ESCALATION = TABLE)
GO
COMMIT*/

