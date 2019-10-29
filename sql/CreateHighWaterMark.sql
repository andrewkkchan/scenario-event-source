USE [oi_outbound_db2]
GO

/****** Object:  Table [dbo].[account_category_change]    Script Date: 9/13/2019 11:23:51 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
DROP TABLE [dbo].[high_water_mark];
CREATE TABLE [dbo].[high_water_mark](
	[high_water_mark] [bigint] NOT NULL,
 CONSTRAINT [pk_high_water_mark] PRIMARY KEY CLUSTERED 
(
	[high_water_mark] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[high_water_mark] (high_water_mark) VALUES (0);
GO


