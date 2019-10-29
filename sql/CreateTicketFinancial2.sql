USE [oi_outbound_db2]
GO

/****** Object:  Table [dbo].[ticket_bet_financial]    Script Date: 9/16/2019 9:55:50 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[ticket_bet_financial_2]

CREATE TABLE [dbo].[ticket_bet_financial_2](
	[ticket_id] [varchar](30) NOT NULL,
	[bet_id] [int] NOT NULL,
	[leg_no] [int] NOT NULL,
	[pool_id] [int] NOT NULL,
	[odds_sold] [decimal](28, 12)  NULL,
	[liability] [decimal](28, 12)  NULL,
	[investment] [decimal](28, 12) NULL,
	[odds_paid] [decimal](28, 12) NULL,
	[dividend] [decimal](28, 12) NULL,
	[refund] [decimal](28, 12) NULL,
	[pool_payout_status] [tinyint] NULL,
	[is_paid] [bit] NULL,
	[payout_datetime] [datetime2](7) NULL,
	[is_settled] [bit] NULL,
	[settlement_datetime] [datetime2](7) NULL,
	[is_closed] [bit] NULL,
	[close_datetime] [int] NULL,
	[odds_ref] [decimal](28, 12) NULL,
	[value_score] [decimal](28, 12) NULL,
	[user_flag] [tinyint] NULL,
	[is_large] [bit] NULL,
	[display_rule_id] [int] NULL,
	[last_modified_datetime] [datetime2](7) NULL,
	[scenario] [varchar](30) NOT NULL
 CONSTRAINT [pk_ticket_bet_financial_2] PRIMARY KEY CLUSTERED 
(
	[ticket_id] ASC,
	[leg_no] ASC,
	[scenario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


