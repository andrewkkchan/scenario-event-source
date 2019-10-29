using OIData;
using System;
using OIFinancialUpdateEventSource.Consumer;
using OIFinancialUpdateEventSource.Mutator;
using OIFinancialUpdateEventSource.Sourcer;
using System.Threading;
using System.Collections.Generic;

namespace OIFinancialUpdateEventSource.Processor
{
    public class SingleProcessor : IBetProcessor
    {
        readonly IMutator mutator;
        readonly IPnLConsumer pnLConsumer;
        readonly ISourcer sourcer;

        public SingleProcessor(IMutator mutator, IPnLConsumer pnLConsumer, ISourcer sourcer)
        {
            this.mutator = mutator;
            this.pnLConsumer = pnLConsumer;
            this.sourcer = sourcer;
        }


        public IDictionary<int, PnLState> Process(int poolSize)
        {

            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Event Sourcing : first pool 71000001 win and second pool 71000004 lose");
            //A. Receive 1 million bets and win the pool

            for (int i = 1; i < poolSize + 1; i++)
            {
                // 1. Flowing state in the form of ticket bets coming in
                SingleTicket ticket = new SingleTicket
                {
                    TicketId = "17_2_1_" + (410100000 + i).ToString(),
                    BetId = 1,
                    LegNo = 1,
                    Odds = 1.5m,
                    UnitBets = 0.1m,
                    PoolId = 71000001
                };

                //2. Produce all Ticket Mutations on PnL based on Pool's Win Scenario
                Mutation ticketMutationWin = mutator.GetTicketMutation(ticket, Result.WIN);
                Mutation ticketMutationLoss = mutator.GetTicketMutation(ticket, Result.LOSS);
                Mutation ticketMutationDraw = mutator.GetTicketMutation(ticket, Result.DRAW);
                Mutation ticketMutationRefund = mutator.GetTicketMutation(ticket, Result.REFUND);

                //3. Source the events
                sourcer.Source(ticketMutationWin);
                sourcer.Source(ticketMutationLoss);
                sourcer.Source(ticketMutationDraw);
                sourcer.Source(ticketMutationRefund);

                //4. Apply the mutation on the system state
                pnLConsumer.Consume(ticketMutationWin);
                pnLConsumer.Consume(ticketMutationLoss);
                pnLConsumer.Consume(ticketMutationDraw);
                pnLConsumer.Consume(ticketMutationRefund);
            }

            //5. Pool Result arrives and is consumed
            Mutation matchMutation = mutator.GetMatchMutation(Result.WIN, 71000001);
            sourcer.Source(matchMutation);
            pnLConsumer.Consume(matchMutation);

            //6. Query Dividend
            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Aggregatged Dividend for pool 71000001 : " + pnLConsumer.QueryDividend(71000001));
            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Aggregatged Refund for pool 71000001 : " + pnLConsumer.QueryRefund(71000001));
            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " High Water Mark Emitted : " + mutator.HighWaterMark);
            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " High Water Mark Processed : " + pnLConsumer.GetHighWaterMark());

            ////B. Receive 1 million bets and lose the pool

            for (int i = poolSize + 1; i < (poolSize * 2) + 1; i++)
            {
                // 1. Flowing state in the form of ticket bets coming in
                SingleTicket ticket = new SingleTicket
                {
                    TicketId = "17_2_1_" + (410100000 + i).ToString(),
                    BetId = 1,
                    LegNo = 1,
                    Odds = 1.5m,
                    UnitBets = 0.1m,
                    PoolId = 71000004
                };

                //2. Produce all Ticket Mutations on PnL based on Pool's Win Scenario
                Mutation ticketMutationWin = mutator.GetTicketMutation(ticket, Result.WIN);
                Mutation ticketMutationLoss = mutator.GetTicketMutation(ticket, Result.LOSS);
                Mutation ticketMutationDraw = mutator.GetTicketMutation(ticket, Result.DRAW);
                Mutation ticketMutationRefund = mutator.GetTicketMutation(ticket, Result.REFUND);

                //3. Source the events
                sourcer.Source(ticketMutationWin);
                sourcer.Source(ticketMutationLoss);
                sourcer.Source(ticketMutationDraw);
                sourcer.Source(ticketMutationRefund);

                //4. Apply the mutation on the system state
                pnLConsumer.Consume(ticketMutationWin);
                pnLConsumer.Consume(ticketMutationLoss);
                pnLConsumer.Consume(ticketMutationDraw);
                pnLConsumer.Consume(ticketMutationRefund);
            }

            //5. Pool Result arrives and is consumed
            pnLConsumer.Consume(mutator.GetMatchMutation(Result.LOSS, 71000004));

            //6. Query Dividend
            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Aggregatged Dividend Payout for pool 71000004 : " + pnLConsumer.QueryDividend(71000004));
            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Aggregatged Refund Payout for pool 71000004 : " + pnLConsumer.QueryRefund(71000004));
            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " High Water Mark Emitted : " + mutator.HighWaterMark);
            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " High Water Mark Processed : " + pnLConsumer.GetHighWaterMark());
            return pnLConsumer.GetPnL();

        }
    }
}
