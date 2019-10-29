-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
USE [oi_outbound_db2]
GO
/** Object:  StoredProcedure [dbo].[usp_ticket_bet_financial_update]    Script Date: 8/21/2019 4:21:22 PM **/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[usp_ticket_bet_financial_update6]
(		
	--Input Parameters
	@pool_id		BIGINT,

	--Output Parameters
	@return_msg             NVARCHAR(500) =''		OUTPUT
)
AS
BEGIN
	-- Set NOCOUNT ON for performance
	SET NOCOUNT ON;

	-- Declare System variables
	DECLARE @tran_count		INT = 0
	DECLARE @row_count		INT = 0
	DECLARE @now			DATETIME2 = [dbo].udfn_get_business_datetime2()
	DECLARE @return_code	INT = 0

	DECLARE @payout_datetime DATETIME2;

	-- ResultStatus
	DECLARE @result_none   TINYINT = 0;
	DECLARE @result_na     TINYINT = 1;
	DECLARE @result_win    TINYINT = 2;
	DECLARE @result_refund TINYINT = 3;
	DECLARE @result_draw   TINYINT = 4;
	DECLARE @result_lost   TINYINT = 5;

	-- WageringStagus
	DECLARE @payout_started	TINYINT = 3;

	-- Validate non-nullable parameters
	IF @pool_id IS NULL
		BEGIN
			SELECT @return_code = -999, 
				   @return_msg = 'pool_id is null'
			RETURN @return_code
		END

	-- Validate pool has started payout
	IF NOT EXISTS(SELECT 1 FROM [dbo].[pool] WHERE [pool_id]=@pool_id AND [wagering_status]=@payout_started AND [payout_started_datetime] IS NOT NULL)
		BEGIN
			SELECT @return_code = -999, 
				   @return_msg = 'pool has not started payout'
			RETURN @return_code
		END

	SELECT @payout_datetime = [payout_started_datetime] FROM [dbo].[pool] WHERE [pool_id] =  @pool_id

    BEGIN TRY
		-- Create Temp Tables
		/*
		IF OBJECT_ID('tempdb..#t_calculate_dividend_ticket_bet_leg') IS NOT NULL DROP TABLE #t_calculate_dividend_ticket_bet_leg
			CREATE TABLE #t_calculate_dividend_ticket_bet_leg (
				[ticket_id]			VARCHAR(30),
				[bet_id]			INT,
				[pool_id]			BIGINT,
				[line_id]			SMALLINT,
				[combination_id]	INT,
				[odds]				DECIMAL(28,12),
				[is_refunded]		BIT,
				[investment]		DECIMAL(28,12),
				[is_allup]			BIT,
				PRIMARY KEY ([ticket_id], [bet_id]))
				
		IF OBJECT_ID('tempdb..#t_calculate_dividend_ticket_bet_financial') IS NOT NULL DROP TABLE #t_calculate_dividend_ticket_bet_financial
			CREATE TABLE #t_calculate_dividend_ticket_bet_financial (
				[ticket_id]				VARCHAR(30),
				[bet_id]				INT,
				[odds_paid]				DECIMAL(28,12),
				[dividend]				DECIMAL(28,12),
				[refund]				DECIMAL(28,12),
				[pool_payout_status]	TINYINT
				PRIMARY KEY ([ticket_id], [bet_id]))

		IF OBJECT_ID('tempdb..#t_ticket_bet_financial') IS NOT NULL DROP TABLE #t_ticket_bet_financial
			CREATE TABLE #t_ticket_bet_financial (
				[ticket_id]				VARCHAR(30),
				[bet_id]				INT,				
				[investment]			DECIMAL(28,12)
				PRIMARY KEY ([ticket_id], [bet_id])	)			
		*/

		-- Update Financials for Allup Tickets
		UPDATE [dbo].[ticket_bet_financial] SET
				[odds_paid]=[SOURCE].[odds_paid],
				[dividend]=[SOURCE].[dividend],
				[refund]=[SOURCE].[refund],
				[pool_payout_status]=[SOURCE].[pool_payout_status],
				[is_paid]=1,
				[payout_datetime]=@payout_datetime,
				[last_modified_datetime]=@now
		FROM [dbo].[ticket_bet_financial] AS [TARGET]
			INNER JOIN (
		-- Calculate Dividends
		-- from single tickets
		SELECT
			 [bet].[ticket_id]				AS [ticket_id]
			,[bet].[bet_id]					AS [bet_id]
			-- Calculate Odds Paid
			,CASE
				WHEN [comb].[result_status]=@result_win		AND [line].[is_split_line]=0								THEN [bet].[odds]
				WHEN [comb].[result_status]=@result_win		AND [line].[is_split_line]=1								THEN [bet].[odds] * 0.5
				ELSE 0.0
			END										AS [odds_paid]
			,CASE
				WHEN [comb].[result_status]=@result_win		AND [line].[is_split_line]=0								THEN [bet].[odds]
				WHEN [comb].[result_status]=@result_win		AND [line].[is_split_line]=1								THEN [bet].[odds] * 0.5
				ELSE 0.0
			END * [bet].[unit_bet]			AS [dividend]
			-- Calculate Refund
			,CASE
				WHEN [comb].[result_status]=@result_draw	AND [line].[is_split_line]=0								THEN 1.0
				WHEN [comb].[result_status]=@result_refund	AND [line].[is_split_line]=0								THEN 1.0
				WHEN [comb].[result_status]=@result_lost	AND [line].[is_split_line]=0	AND [bet].[is_refunded]=1	THEN 1.0
				WHEN [comb].[result_status]=@result_draw	AND [line].[is_split_line]=1								THEN 0.5
				WHEN [comb].[result_status]=@result_refund	AND [line].[is_split_line]=1								THEN 0.5
				WHEN [comb].[result_status]=@result_lost	AND [line].[is_split_line]=1	AND [bet].[is_refunded]=1	THEN 0.5
				ELSE 0.0
			END	* [bet].[unit_bet]			AS [refund]
			-- Determine Pool Payout Status
			,CASE
				WHEN [pool].[payout_started_datetime] IS NOT NULL AND [pool].[wagering_status]=@payout_started THEN 1	-- All Pools Payout Started
				ELSE 3																									-- No Pools Payout Started
			END										AS [pool_payout_status]
		FROM [dbo].[ticket_bet_single] AS [bet]
			INNER JOIN [dbo].[pool_combination] AS [comb]
				ON  [bet].[pool_id] = [comb].[pool_id]
				AND [bet].[pool_id] = @pool_id
				AND [bet].[line_id] = [comb].[line_id]
				AND [bet].[combination_id] = [comb].[combination_id]
			INNER JOIN [dbo].[pool_line] AS [line]
				ON  [line].[pool_id] = [comb].[pool_id]
				AND [line].[line_id] = [comb].[line_id]
			INNER JOIN [dbo].[pool] AS [pool]
				ON [pool].[pool_id] = [comb].[pool_id]
		UNION
		-- from all-up tickets
		SELECT
			 [bet].[ticket_id]				AS [ticket_id]
			,[bet].[bet_id]					AS [bet_id]
			-- Calculate Odds Paid
			,CASE 
				WHEN MIN(
					CASE
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=0 THEN [bet].[odds]
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=1 THEN [bet].[odds] * 0.5
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=1 THEN 0.5
						ELSE 0.0
					END) = 0.0 THEN 0.0
				ELSE
					EXP(SUM(LOG(ABS(
					CASE
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=0 THEN [bet].[odds]
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=1 THEN [bet].[odds] * 0.5
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=1 THEN 0.5
						ELSE null
					END))))
			END							AS [odds_paid]
			,CASE 
				WHEN MIN(
					CASE
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=0 THEN [bet].[odds]
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=1 THEN [bet].[odds] * 0.5
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=1 THEN 0.5
						ELSE 0.0
					END) = 0.0 THEN 0.0
				ELSE			
					EXP(SUM(LOG(ABS(
					CASE
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=0 THEN [bet].[odds]
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=1 THEN [bet].[odds] * 0.5
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=1 THEN 0.5
						ELSE null
					END)))) * [bet].[unit_bet]
			END							AS [dividend]			
			,0.0						AS [refund]
			-- Determine Pool Payout Status
			,CASE
				WHEN SUM(CASE WHEN [pool].[payout_started_datetime] IS NOT NULL AND [pool].[wagering_status]=@payout_started THEN 1 ELSE 0 END) = COUNT([bet].[bet_id]) THEN 1	-- All Pools Payout Started
				WHEN SUM(CASE WHEN [pool].[payout_started_datetime] IS NOT NULL AND [pool].[wagering_status]=@payout_started THEN 1 ELSE 0 END) = 0						THEN 3	-- No Pools Payout Started
				WHEN SUM(CASE WHEN [pool].[payout_started_datetime] IS NOT NULL AND [pool].[wagering_status]=@payout_started THEN 1 ELSE 0 END) < COUNT([bet].[bet_id]) THEN 2	-- Partial Pools Payout Started
			END											AS [pool_payout_status]
		FROM [dbo].[ticket_bet_allup] AS [bet]
			INNER JOIN [dbo].[pool_combination] AS [comb]
				ON  [bet].[pool_id] = [comb].[pool_id]
				AND [bet].[pool_id] = @pool_id
				AND [bet].[line_id] = [comb].[line_id]
				AND [bet].[combination_id] = [comb].[combination_id]
			INNER JOIN [dbo].[pool_line] AS [line]
				ON  [line].[pool_id] = [comb].[pool_id]
				AND [line].[line_id] = [comb].[line_id]
			INNER JOIN [dbo].[pool] AS [pool]
				ON [pool].[pool_id] = [comb].[pool_id]
		GROUP BY [bet].[ticket_id], [bet].[bet_id], [bet].[unit_bet]
		) AS [SOURCE]
				ON  [SOURCE].[ticket_id] = [TARGET].[ticket_id]
				AND [SOURCE].[bet_id] = [TARGET].[bet_id]
				
		--Determine impacted financial ticket bets
		
		/*
		INSERT INTO #t_ticket_bet_financial
		SELECT 
			[financial].[ticket_id]
			,[financial].[bet_id]
			,[financial].[investment]
		FROM [dbo].[ticket_bet_financial] AS [financial]
			INNER JOIN #t_calculate_dividend_ticket_bet_leg [bet]
				ON [bet].[ticket_id] = [financial].[ticket_id]
				AND [bet].[bet_id] = [financial].[bet_id]	
		*/
		
		/*
		-- Calculate Dividend for Single Tickets
		INSERT INTO #t_calculate_dividend_ticket_bet_financial
		SELECT
			 [bet].[ticket_id]				AS [ticket_id]
			,[bet].[bet_id]					AS [bet_id]
			-- Calculate Odds Paid
			,CASE
				WHEN [comb].[result_status]=@result_win		AND [line].[is_split_line]=0								THEN [bet].[odds]
				WHEN [comb].[result_status]=@result_win		AND [line].[is_split_line]=1								THEN [bet].[odds] * 0.5
				ELSE 0.0
			END										AS [odds_paid]
			,CASE
				WHEN [comb].[result_status]=@result_win		AND [line].[is_split_line]=0								THEN [bet].[odds]
				WHEN [comb].[result_status]=@result_win		AND [line].[is_split_line]=1								THEN [bet].[odds] * 0.5
				ELSE 0.0
			END * [bet].[investment]			AS [dividend]
			-- Calculate Refund
			,CASE
				WHEN [comb].[result_status]=@result_draw	AND [line].[is_split_line]=0								THEN 1.0
				WHEN [comb].[result_status]=@result_refund	AND [line].[is_split_line]=0								THEN 1.0
				WHEN [comb].[result_status]=@result_lost	AND [line].[is_split_line]=0	AND [bet].[is_refunded]=1	THEN 1.0
				WHEN [comb].[result_status]=@result_draw	AND [line].[is_split_line]=1								THEN 0.5
				WHEN [comb].[result_status]=@result_refund	AND [line].[is_split_line]=1								THEN 0.5
				WHEN [comb].[result_status]=@result_lost	AND [line].[is_split_line]=1	AND [bet].[is_refunded]=1	THEN 0.5
				ELSE 0.0
			END	* [bet].[investment]			AS [refund]
			-- Determine Pool Payout Status
			,CASE
				WHEN [pool].[payout_started_datetime] IS NOT NULL AND [pool].[wagering_status]=@payout_started THEN 1	-- All Pools Payout Started
				ELSE 3																									-- No Pools Payout Started
			END										AS [pool_payout_status]
		FROM #t_calculate_dividend_ticket_bet_leg AS [bet]
			INNER JOIN [dbo].[pool_combination] AS [comb]
				ON  [bet].[pool_id] = [comb].[pool_id]
				AND [bet].[line_id] = [comb].[line_id]
				AND [bet].[combination_id] = [comb].[combination_id]
				AND [bet].[is_allup] = 0
			INNER JOIN [dbo].[pool_line] AS [line]
				ON  [line].[pool_id] = [comb].[pool_id]
				AND [line].[line_id] = [comb].[line_id]
			INNER JOIN [dbo].[pool] AS [pool]
				ON [pool].[pool_id] = [comb].[pool_id]
		UNION
		-- Calculate Dividend for Allup Tickets
		SELECT
			 [bet].[ticket_id]					AS [ticket_id]
			,[bet].[bet_id]						AS [bet_id]
			-- Calculate Odds Paid
			,CASE 
				WHEN MIN(
					CASE
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=0 THEN [bet].[odds]
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=1 THEN [bet].[odds] * 0.5
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=1 THEN 0.5
						ELSE 0.0
					END) = 0.0 THEN 0.0
				ELSE
					EXP(SUM(LOG(ABS(
					CASE
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=0 THEN [bet].[odds]
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=1 THEN [bet].[odds] * 0.5
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=1 THEN 0.5
						ELSE null
					END))))
			END							AS [odds_paid]
			,CASE 
				WHEN MIN(
					CASE
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=0 THEN [bet].[odds]
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=1 THEN [bet].[odds] * 0.5
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=1 THEN 0.5
						ELSE 0.0
					END) = 0.0 THEN 0.0
				ELSE			
					EXP(SUM(LOG(ABS(
					CASE
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=0 THEN [bet].[odds]
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=0 THEN 1.0
						WHEN [comb].[result_status]=@result_win									AND [line].[is_split_line]=1 THEN [bet].[odds] * 0.5
						WHEN [comb].[result_status]=@result_draw								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_refund								AND [line].[is_split_line]=1 THEN 0.5
						WHEN [comb].[result_status]=@result_lost	AND [bet].[is_refunded]=1	AND [line].[is_split_line]=1 THEN 0.5
						ELSE null
					END)))) * [bet].[investment]
			END							AS [dividend]			
			,0.0						AS [refund]
			-- Determine Pool Payout Status
			,CASE
				WHEN SUM(CASE WHEN [pool].[payout_started_datetime] IS NOT NULL AND [pool].[wagering_status]=@payout_started THEN 1 ELSE 0 END) = COUNT([bet].[bet_id]) THEN 1	-- All Pools Payout Started
				WHEN SUM(CASE WHEN [pool].[payout_started_datetime] IS NOT NULL AND [pool].[wagering_status]=@payout_started THEN 1 ELSE 0 END) = 0						THEN 3	-- No Pools Payout Started
				WHEN SUM(CASE WHEN [pool].[payout_started_datetime] IS NOT NULL AND [pool].[wagering_status]=@payout_started THEN 1 ELSE 0 END) < COUNT([bet].[bet_id]) THEN 2	-- Partial Pools Payout Started
			END											AS [pool_payout_status]
		FROM #t_calculate_dividend_ticket_bet_leg AS [bet]
			INNER JOIN [dbo].[pool_combination] AS [comb]
				ON  [bet].[pool_id] = [comb].[pool_id]
				AND [bet].[line_id] = [comb].[line_id]
				AND [bet].[combination_id] = [comb].[combination_id]
				AND [bet].[is_allup] = 1
			INNER JOIN [dbo].[pool_line] AS [line]
				ON  [line].[pool_id] = [comb].[pool_id]
				AND [line].[line_id] = [comb].[line_id]
			INNER JOIN [dbo].[pool] AS [pool]
				ON [pool].[pool_id] = [comb].[pool_id]
		GROUP BY [bet].[ticket_id], [bet].[bet_id], [bet].[investment]
		*/

		/*
		-- Update Financials for Allup Tickets
		UPDATE [dbo].[ticket_bet_financial] SET
				[odds_paid]=[SOURCE].[odds_paid],
				[dividend]=[SOURCE].[dividend],
				[refund]=[SOURCE].[refund],
				[pool_payout_status]=[SOURCE].[pool_payout_status],
				[is_paid]=1,
				[payout_datetime]=@payout_datetime,
				[last_modified_datetime]=@now
		FROM [dbo].[ticket_bet_financial] AS [TARGET]
			INNER JOIN #t_calculate_dividend_ticket_bet_financial AS [SOURCE]
				ON  [SOURCE].[ticket_id] = [TARGET].[ticket_id]
				AND [SOURCE].[bet_id] = [TARGET].[bet_id]
		*/

		IF OBJECT_ID('tempdb..#t_calculate_dividend_ticket_bet_leg') IS NOT NULL DROP TABLE #t_calculate_dividend_ticket_bet_leg				
		IF OBJECT_ID('tempdb..#t_calculate_dividend_ticket_bet_financial') IS NOT NULL DROP TABLE #t_calculate_dividend_ticket_bet_financial
		IF OBJECT_ID('tempdb..#t_ticket_bet_financial') IS NOT NULL DROP TABLE #t_ticket_bet_financial

        SELECT @return_msg   = ''
    END TRY

	-- Error Handling
	BEGIN CATCH
		BEGIN
			--drop temp table
			IF OBJECT_ID('tempdb..#t_calculate_dividend_ticket_bet_leg') IS NOT NULL DROP TABLE #t_calculate_dividend_ticket_bet_leg				
			IF OBJECT_ID('tempdb..#t_calculate_dividend_ticket_bet_financial') IS NOT NULL DROP TABLE #t_calculate_dividend_ticket_bet_financial
			IF OBJECT_ID('tempdb..#t_ticket_bet_financial') IS NOT NULL DROP TABLE #t_ticket_bet_financial

			SET @return_code = ERROR_NUMBER()
			SET @return_msg  = SUBSTRING('ERROR in ' + ERROR_PROCEDURE() + ' line ' + CONVERT(VARCHAR(100),ERROR_LINE()) + ': ' + ERROR_MESSAGE(), 1, 500)
		END
	END CATCH

    RETURN @return_code
END