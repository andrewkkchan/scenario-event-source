using OIData;

namespace OIFinancialUpdateEventSource.Mutator
{
    public interface IMutator
    {
        int HighWaterMark { get; set; }
        Mutation GetTicketMutation(SingleTicket ticket, Result result);
        Mutation GetMatchMutation(Result result, int poolId);

    }
    public class InMemoryMutator: IMutator
    {
        public int HighWaterMark { get; set; }

        public Mutation GetTicketMutation(SingleTicket ticket, Result result)
        {
            Mutation ticketMutation = new Mutation
            {
                MutationType = MutationType.Ticket,
                SequenceNumber = HighWaterMark++,
                TicketId = ticket.TicketId,
                BetId = ticket.BetId,
                LegNo = ticket.LegNo,
                PoolId = ticket.PoolId,
                Result = result,
                OddsSold = ticket.Odds,
                Investment = ticket.UnitBets
            };

            if (result == Result.WIN)
            {
                ticketMutation.OddsPaid = ticketMutation.OddsSold;
                ticketMutation.Dividend = ticketMutation.OddsPaid * ticketMutation.Investment;
                ticketMutation.Refund = 0;
            }

            if (result == Result.LOSS)
            {
                ticketMutation.OddsPaid = 0;
                ticketMutation.Dividend = 0;
                ticketMutation.Refund = 0;
            }

            if (result == Result.REFUND)
            {
                ticketMutation.OddsPaid = 0;
                ticketMutation.Dividend = 0;
                ticketMutation.Refund = ticketMutation.Investment;
            }

            if (result == Result.DRAW)
            {
                ticketMutation.OddsPaid = 0;
                ticketMutation.Dividend = 0;
                ticketMutation.Refund = ticketMutation.Investment;
            }
            return ticketMutation;
        }

        public Mutation GetMatchMutation(Result result, int poolId)
        {
            Mutation mutation = new Mutation
            {
                MutationType = MutationType.Match,
                SequenceNumber = HighWaterMark++,
                PoolId = poolId,
                Result = result
            };
            return mutation;
        }
    }
}
