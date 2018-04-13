/****** Object:  Table [dbo].[DS_SchemaESDFunctionServiceLink]    Script Date: 03/06/2014 14:59:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DS_SchemaESDFunctionServiceLink]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DS_SchemaESDFunctionServiceLink](
	[LinkId] [int] IDENTITY(1,1) NOT NULL,
	[SchemaId] [int] NULL,
	[EsdFunctionServiceId] [nvarchar](50) NULL,
 CONSTRAINT [PK_DS_SchemaESDFunctionServiceLink_1] PRIMARY KEY CLUSTERED 
(
	[LinkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[SYS_DS_ConfigurationSettings]    Script Date: 03/07/2014 11:40:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SYS_DS_ConfigurationSettings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SYS_DS_ConfigurationSettings](
	[SettingId] [int] NOT NULL,
	[CouncilName] [nvarchar](512) NULL,
	[CouncilUrl] [nvarchar](512) NULL,
	[CouncilUri] [nvarchar](1024) NULL,
	[MapCentreLatitude] [nvarchar](50) NULL,
	[MapCentreLongitude] [nvarchar](50) NULL,
	[MapDefaultZoom] [nvarchar](10) NULL,
	[AnalyticsTrackingRef] [nvarchar](255) NULL,
	
 CONSTRAINT [PK_SYS_DS_ConfigurationSettings] PRIMARY KEY CLUSTERED 
(
	[SettingId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


/*
   28 April 201414:41:09
   User: 
   Server: clesql01
   Database: DataShare
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/

IF NOT EXISTS(SELECT * FROM sys.columns 
        WHERE [name] = N'IsStandardisedSchemaUrl' AND [object_id] = OBJECT_ID(N'[dbo].[DS_DataSetSchema]'))
Begin
ALTER TABLE dbo.DS_DataSetSchema ADD
	IsStandardisedSchemaUrl bit NULL
	end
GO



/*
   10 April 201415:35:20
   User: 
   Server: ICTPC14696\SQLEXPRESS
   Database: DataShare71a
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/

IF NOT EXISTS(SELECT * FROM sys.columns WHERE [name] = N'SchemaDefinitionFromUrl' AND [object_id] = OBJECT_ID(N'[dbo].[DS_DataSetSchema]'))
	begin
	ALTER TABLE dbo.DS_DataSetSchema ADD
		SchemaDefinitionFromUrl nvarchar(2048) NULL
		end
		
GO
/*ALTER TABLE dbo.DS_DataSetSchema SET (LOCK_ESCALATION = TABLE)
GO
COMMIT*/




/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
IF NOT EXISTS(SELECT * FROM sys.columns 
        WHERE [name] = N'CouncilSpatialUri' AND [object_id] = OBJECT_ID(N'[dbo].[SYS_DS_ConfigurationSettings]'))
begin
ALTER TABLE dbo.SYS_DS_ConfigurationSettings ADD
	CouncilSpatialUri nvarchar(1024) NULL
end
GO

