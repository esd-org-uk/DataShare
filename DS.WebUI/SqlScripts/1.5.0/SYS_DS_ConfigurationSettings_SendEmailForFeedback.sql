

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

IF NOT EXISTS(SELECT * FROM sys.columns 
		WHERE [name] = N'SendEmailForFeedBack' 
		AND [object_id] = OBJECT_ID(N'SYS_DS_ConfigurationSettings')        
		)
  
BEGIN
	-- Column Not Exists
ALTER TABLE dbo.SYS_DS_ConfigurationSettings ADD
	SendEmailForFeedBack nvarchar(255) NULL
	--,	SmtpServer nvarchar(255) NULL,
	--SmtpUsername nvarchar(50) NULL,
	--SmtpPassword nvarchar(50) NULL
END
IF NOT EXISTS(SELECT * FROM sys.columns 
		WHERE [name] = N'SmtpServer' 
		AND [object_id] = OBJECT_ID(N'SYS_DS_ConfigurationSettings')        
		)
BEGIN
	-- Column Not Exists
ALTER TABLE dbo.SYS_DS_ConfigurationSettings ADD
	SmtpServer nvarchar(255) NULL
END
IF NOT EXISTS(SELECT * FROM sys.columns 
		WHERE [name] = N'SmtpUsername' 
		AND [object_id] = OBJECT_ID(N'SYS_DS_ConfigurationSettings')        
		)
BEGIN
	-- Column Not Exists
ALTER TABLE dbo.SYS_DS_ConfigurationSettings ADD
	SmtpUsername nvarchar(50) NULL
END

IF NOT EXISTS(SELECT * FROM sys.columns 
		WHERE [name] = N'SmtpPassword' 
		AND [object_id] = OBJECT_ID(N'SYS_DS_ConfigurationSettings')        
		)
BEGIN
	-- Column Not Exists
ALTER TABLE dbo.SYS_DS_ConfigurationSettings ADD
	SmtpPassword nvarchar(50) NULL
END
COMMIT


