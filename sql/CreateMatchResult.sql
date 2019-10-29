USE [oi_outbound_db2]
GO

/****** Object:  Table [dbo].[account_category_change]    Script Date: 9/13/2019 11:23:51 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
DROP TABLE [dbo].[match_result]
CREATE TABLE [dbo].[match_result](
	[pool_id] [int] NOT NULL,
	[scenario] [varchar](30) NOT NULL,
 CONSTRAINT [pk_match_result] PRIMARY KEY CLUSTERED 
(
	[pool_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[match_result] (pool_id, scenario) VALUES (71000001, 'UNKNOWN');
INSERT INTO [dbo].[match_result] (pool_id, scenario) VALUES (71000002, 'UNKNOWN');
INSERT INTO [dbo].[match_result] (pool_id, scenario) VALUES (71000003, 'UNKNOWN');
INSERT INTO [dbo].[match_result] (pool_id, scenario) VALUES (71000004, 'UNKNOWN');
INSERT INTO [dbo].[match_result] (pool_id, scenario) VALUES (71000005, 'UNKNOWN');
INSERT INTO [dbo].[match_result] (pool_id, scenario) VALUES (71000006, 'UNKNOWN');
INSERT INTO [dbo].[match_result] (pool_id, scenario) VALUES (71000007, 'UNKNOWN');
INSERT INTO [dbo].[match_result] (pool_id, scenario) VALUES (71000008, 'UNKNOWN');



