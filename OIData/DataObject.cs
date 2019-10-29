using System;
using System.Collections.Generic;
using System.Text;

namespace OIData
{
    public class SingleTicket
    {
        public string TicketId { get; set; }
        public int BetId { get; set; }
        public int LegNo { get; set; }
        public int PoolId { get; set; }
        public int LineId { get; set; }
        public int CombinationId { get; set; }
        public int SellingStage { get; set; }
        public decimal Odds { get; set; }
        public decimal UnitBets { get; set; }

        public bool IsRefunded { get; set; }
        public decimal  CounterOdds { get; set; }
        public decimal SportsId { get; set; }
        public DateTime CreateDataTime { get; set; }

    }

    public class AllUpTicket
    {
        public string TicketId { get; set; }
        public int BetId { get; set; }
        public List<Leg> Legs { get; set; }
        public decimal UnitBets { get; set; }
        public bool IsRefunded { get; set; }
        public decimal CounterOdds { get; set; }
        public decimal SportsId { get; set; }
        public DateTime CreateDataTime { get; set; }
        public int LegsPlayed { get; set; }
    }

    public class Leg
    {
        public int PoolId { get; set; }

        public int CombinationId { get; set; }
        public decimal Odds { get; set; }
    }

    public enum MutationType
    {
        Ticket, Match
    }

    public enum Result
    {
        WIN, LOSS, DRAW, REFUND
    }

    public class MatchResult
    {
        public int PoolId { get; set; }
        public int CombinationId { get; set; }
        public Result Result { get; set; }
    }
    public class Mutation
    {
        public MutationType MutationType { get; set; }
        public int SequenceNumber { get; set; }
        public string TicketId { get; set; }
        public int BetId { get; set; }
        public int LegNo { get; set; }
        public decimal Investment { get; set; }
        public decimal OddsPaid { get; set; }
        public decimal OddsSold { get; set; }
        public decimal Dividend { get; set; }
        public decimal Refund { get; set; }
        public Result Result { get; set; }
        public int PoolId { get; set; }
    }

    public class PnLState
    {
        public decimal AggregatedDividend { get; set; }
        public decimal AggregatedRefund { get; set; }
        public decimal AggregatedInvestment { get; set; }
    }

}
