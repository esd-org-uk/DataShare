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