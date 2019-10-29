USE [oi_outbound_db2]
GO

/****** Object:  Table [dbo].[account_category_change]    Script Date: 9/13/2019 11:23:51 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
DROP TABLE [dbo].[pnl_state]
CREATE TABLE [dbo].[pnl_state](
	[pool_id] [int] NOT NULL,
	[aggregated_dividend] [decimal](28,12) NOT NULL,
	[aggregated_refund] [decimal](28,12) NOT NULL,
	[scenario] [varchar](30) NOT NULL,
 CONSTRAINT [pk_pnl_state] PRIMARY KEY CLUSTERED 
(
	[pool_id] ASC, 
	[scenario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000001, 0, 0, 'WIN');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000001, 0, 0, 'LOSS');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000001, 0, 0, 'DRAW');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000001, 0, 0, 'REFUND');

INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000002, 0, 0, 'WIN');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000002, 0, 0, 'LOSS');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000002, 0, 0, 'DRAW');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000002, 0, 0, 'REFUND');

INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000003, 0, 0, 'WIN');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000003, 0, 0, 'LOSS');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000003, 0, 0, 'DRAW');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000003, 0, 0, 'REFUND');

INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000004, 0, 0, 'WIN');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000004, 0, 0, 'LOSS');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000004, 0, 0, 'DRAW');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000004, 0, 0, 'REFUND');

INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000005, 0, 0, 'WIN');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000005, 0, 0, 'LOSS');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000005, 0, 0, 'DRAW');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000005, 0, 0, 'REFUND');

INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000006, 0, 0, 'WIN');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000006, 0, 0, 'LOSS');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000006, 0, 0, 'DRAW');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000006, 0, 0, 'REFUND');

INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000007, 0, 0, 'WIN');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000007, 0, 0, 'LOSS');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000007, 0, 0, 'DRAW');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000007, 0, 0, 'REFUND');

INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000008, 0, 0, 'WIN');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000008, 0, 0, 'LOSS');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000008, 0, 0, 'DRAW');
INSERT INTO [dbo].[pnl_state] (pool_id, aggregated_dividend, aggregated_refund, scenario) VALUES (71000008, 0, 0, 'REFUND');


