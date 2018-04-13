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
