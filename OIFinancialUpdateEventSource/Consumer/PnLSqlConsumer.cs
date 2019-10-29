using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using OIData;

namespace OIFinancialUpdateEventSource.Consumer
{
    public class PnLSqlConsumer : IPnLConsumer, IDisposable
    {
        private readonly SqlConnection connection;
        public PnLSqlConsumer()
        {
            string connectionString = "Data Source=ANDREWCHAN0F39;Initial Catalog=oi_outbound_db2;Integrated Security=SSPI;";
            connection = new SqlConnection(connectionString);
            connection.Open();

            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM dbo.pool", connection))
            {
                var result = command.ExecuteScalar();
                Console.WriteLine("Pool Count = " + result);
            }
        }

        public void Consume(Mutation mutation)
        {
            if (mutation.MutationType == MutationType.Ticket)
            {
                using (SqlCommand command = new SqlCommand("UPDATE dbo.pnl_state SET aggregated_dividend = aggregated_dividend + @dividend " +
                    ", aggregated_refund = aggregated_refund + @refund WHERE scenario = @scenario AND pool_id = @poolId", connection))
                {
                    command.Parameters.AddWithValue("@dividend", mutation.Dividend);
                    command.Parameters.AddWithValue("@refund", mutation.Refund);
                    command.Parameters.AddWithValue("@poolId", mutation.PoolId);
                    command.Parameters.AddWithValue("@scenario", Enum.GetName(typeof(Result), mutation.Result));
                    var result = command.ExecuteNonQuery();
                }

                using (SqlCommand command =
                    new SqlCommand("INSERT INTO dbo.ticket_bet_financial_2 " +
                    "(ticket_id, bet_id, leg_no, pool_id, investment, liability, dividend, odds_paid, odds_sold, refund, scenario, pool_payout_status, is_paid, is_settled, is_closed, odds_ref, value_score, is_large) " +
                    "VALUES (@ticket_id, @bet_id, @leg_no, @pool_id, @investment, @liability, @dividend, @oddsPaid, @oddsSold, @refund, @scenario, @poolPayoutStatus,@isPaid, @isSettled, @isClosed, @oddsRef, @valueScore, @isLarge)", connection))
                {
                    command.Parameters.AddWithValue("@scenario", Enum.GetName(typeof(Result), mutation.Result));
                    command.Parameters.AddWithValue("@bet_id", mutation.BetId);
                    command.Parameters.AddWithValue("@leg_no", mutation.LegNo);
                    command.Parameters.AddWithValue("@pool_id", mutation.PoolId);
                    command.Parameters.AddWithValue("@investment", mutation.Investment);
                    command.Parameters.AddWithValue("@liability", mutation.Dividend+ mutation.Refund);
                    command.Parameters.AddWithValue("@dividend", mutation.Dividend);
                    command.Parameters.AddWithValue("@oddsPaid", mutation.OddsPaid);
                    command.Parameters.AddWithValue("@oddsSold", mutation.OddsSold);
                    command.Parameters.AddWithValue("@refund", mutation.Refund);
                    command.Parameters.AddWithValue("@ticket_id", mutation.TicketId);
                    command.Parameters.AddWithValue("@poolPayoutStatus", true);
                    command.Parameters.AddWithValue("@isPaid", true);
                    command.Parameters.AddWithValue("@isSettled", false);
                    command.Parameters.AddWithValue("@isClosed", false);
                    command.Parameters.AddWithValue("@oddsRef", 1.0m);
                    command.Parameters.AddWithValue("@valueScore", 98.5m);
                    command.Parameters.AddWithValue("@isLarge", false);


                    var result = command.ExecuteNonQuery();
                }
            }

            if (mutation.MutationType == MutationType.Match)
            {
                using (SqlCommand command = new SqlCommand("UPDATE dbo.match_result SET scenario = @scenario WHERE pool_id = @poolId", connection))
                {
                    command.Parameters.AddWithValue("@scenario", Enum.GetName(typeof(Result), mutation.Result));
                    command.Parameters.AddWithValue("@poolId", mutation.PoolId);
                    var result = command.ExecuteNonQuery();
                }
            }
            SetHighWaterMark(GetHighWaterMark() + 1);
        }

        public void Dispose()
        {
            connection.Close();
        }

        public decimal QueryDividend(int poolId)
        {
            using (SqlCommand command = new SqlCommand("SELECT aggregated_dividend FROM dbo.pnl_state WHERE pool_id = @poolId AND scenario = " +
                "(SELECT scenario FROM dbo.match_result WHERE pool_id = @poolId)", connection))
            {
                command.Parameters.AddWithValue("@poolId", poolId);
                var result = command.ExecuteScalar();
                return (decimal)result;
            }
        }

        public long GetHighWaterMark()
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM dbo.high_water_mark", connection))
            {
                var result = command.ExecuteScalar();
                return (long)result;
            }
        }

        public void SetHighWaterMark(long value)
        {
            using (SqlCommand command = new SqlCommand("UPDATE dbo.high_water_mark SET high_water_mark= @value", connection))
            {
                command.Parameters.AddWithValue("@value", value);
                var result = command.ExecuteNonQuery();
            }
        }

        public decimal QueryRefund(int poolId)
        {
            using (SqlCommand command = new SqlCommand("SELECT aggregated_refund FROM dbo.pnl_state WHERE pool_id = @poolId AND scenario = " +
                          "(SELECT scenario FROM dbo.match_result WHERE pool_id = @poolId)", connection))
            {
                command.Parameters.AddWithValue("@poolId", poolId);
                var result = command.ExecuteScalar();
                return (decimal)result;
            }
        }

        public void PrintAllDividend()
        {
            for (int poolId = 71000001; poolId < 71000009; poolId ++)
            {
                Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Aggregatged Dividend for pool " + poolId + " : " + QueryDividend(poolId));
            }
        }

        public void PrintAllRefund()
        {
            for (int poolId = 71000001; poolId < 71000009; poolId++)
            {
                Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Aggregatged Refund for pool " + poolId + " : " + QueryRefund(poolId));
            }
        }

        public decimal QueryInvestment(int poolId)
        {
            throw new NotImplementedException();
        }

        public void PrintAllInvestment()
        {
            throw new NotImplementedException();
        }

        public IDictionary<int,PnLState> GetPnL()
        {
            throw new NotImplementedException();
        }
    }
}
