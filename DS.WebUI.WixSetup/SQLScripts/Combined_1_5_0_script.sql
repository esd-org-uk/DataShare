BEGIN TRANSACTION

IF NOT EXISTS(SELECT * FROM sys.columns 
		WHERE [name] = N'DefaultDisplayWeight' AND [object_id] = OBJECT_ID(N'DS_DataSetSchemaColumns'))
BEGIN
ALTER TABLE dbo.DS_DataSetSchemaColumns ADD
	DefaultDisplayWeight int NULL
ALTER TABLE dbo.DS_DataSetSchemaColumns ADD CONSTRAINT
	DF_DS_DataSetSchemaColumns_DefaultDisplayWeight DEFAULT 0 FOR DefaultDisplayWeight
END

COMMIT

BEGIN TRANSACTION

IF NOT EXISTS(SELECT * FROM sys.columns 
		WHERE [name] = N'SendEmailForFeedBack' 
		AND [object_id] = OBJECT_ID(N'SYS_DS_ConfigurationSettings')        
		)
  
BEGIN
ALTER TABLE dbo.SYS_DS_ConfigurationSettings ADD
	SendEmailForFeedBack nvarchar(255) NULL
END
IF NOT EXISTS(SELECT * FROM sys.columns 
		WHERE [name] = N'SmtpServer' 
		AND [object_id] = OBJECT_ID(N'SYS_DS_ConfigurationSettings')        
		)
BEGIN
ALTER TABLE dbo.SYS_DS_ConfigurationSettings ADD
	SmtpServer nvarchar(255) NULL
END
IF NOT EXISTS(SELECT * FROM sys.columns 
		WHERE [name] = N'SmtpUsername' 
		AND [object_id] = OBJECT_ID(N'SYS_DS_ConfigurationSettings')        
		)
BEGIN
ALTER TABLE dbo.SYS_DS_ConfigurationSettings ADD
	SmtpUsername nvarchar(50) NULL
END

IF NOT EXISTS(SELECT * FROM sys.columns 
		WHERE [name] = N'SmtpPassword' 
		AND [object_id] = OBJECT_ID(N'SYS_DS_ConfigurationSettings')        
		)
BEGIN
ALTER TABLE dbo.SYS_DS_ConfigurationSettings ADD
	SmtpPassword nvarchar(50) NULL
END
COMMIT





