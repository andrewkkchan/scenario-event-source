using OIData;
using OIFinancialUpdateEventSource.Consumer;
using OIFinancialUpdateEventSource.Mutator;
using OIFinancialUpdateEventSource.Sourcer;
using System;
using System.Collections.Generic;
using System.Text;

namespace OIFinancialUpdateEventSource.Generator
{
    public interface IScenarioGenerator
    {

        void Generate(IMutator mutator, ISourcer sourcer, IPnLConsumer pnLConsumer, SingleTicket ticket);
        void Pick(IMutator mutator, ISourcer sourcer, IPnLConsumer pnLConsumer, MatchResult matchResult);
    }


    public class ScenarioGenerator : IScenarioGenerator
    {
        public void Generate(IMutator mutator, ISourcer sourcer, IPnLConsumer pnLConsumer, SingleTicket ticket)
        {
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

        public void Pick(IMutator mutator, ISourcer sourcer, IPnLConsumer pnLConsumer, MatchResult matchResult)
        {
            //5. Pool Result arrives and is consumed
            Mutation matchMutationLeg1 = mutator.GetMatchMutation(matchResult.Result, matchResult.PoolId);
            sourcer.Source(matchMutationLeg1);
            pnLConsumer.Consume(matchMutationLeg1);
        }
    }
}
