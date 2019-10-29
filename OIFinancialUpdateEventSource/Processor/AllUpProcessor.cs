using OIData;
using System;
using System.Collections.Generic;
using OIFinancialUpdateEventSource.Consumer;
using OIFinancialUpdateEventSource.Mutator;
using OIFinancialUpdateEventSource.Sourcer;
using System.Threading;
using OIFinancialUpdateEventSource.Reducer;
using OIFinancialUpdateEventSource.Generator;
using OIFinancialUpdateEventSource.EventReceiver;

namespace OIFinancialUpdateEventSource.Processor
{
    public class AllUpProcessor : IBetProcessor
    {
        readonly IMutator mutator;
        readonly IPnLConsumer pnLConsumer;
        readonly ISourcer sourcer;
        readonly List<AllUpTicket> allUpTickets = new List<AllUpTicket>();
        readonly List<MatchResult> matchResults = new List<MatchResult>();
        readonly IReducer reducer;
        readonly IScenarioGenerator scenarioGenerator;
        readonly IEventReceiver eventReceiver;


        public AllUpProcessor(IMutator mutator, IPnLConsumer pnLConsumer, ISourcer sourcer, IReducer reducer, IScenarioGenerator scenarioGenerator, IEventReceiver eventReceiver)
        {
            this.mutator = mutator;
            this.pnLConsumer = pnLConsumer;
            this.sourcer = sourcer;
            this.reducer = reducer;
            this.scenarioGenerator = scenarioGenerator;
            this.eventReceiver = eventReceiver;
            
        }
        public IDictionary<int,PnLState> Process(int poolSize)
        {
            //Process Single Tickets in the memory
            var watch = System.Diagnostics.Stopwatch.StartNew();
            eventReceiver.Receive(allUpTickets, matchResults, poolSize);

            // the code that you want to measure comes here
            watch.Stop();
            Console.WriteLine("Time Elapsed in ms for Receiving events :  " + watch.ElapsedMilliseconds);
            int matchPlayed = 0;

            while (matchResults.Count > matchPlayed)
            {
                watch.Restart();
                Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " After match " + matchPlayed +" All Up Tickets Unprocessed : " + allUpTickets.Count);
                foreach (AllUpTicket allUpTicket in allUpTickets)
                {
                    foreach (Leg leg in allUpTicket.Legs)
                    {
                        if (leg.PoolId == matchResults[matchPlayed].PoolId)
                        {
                            SingleTicket ticket = reducer.Reduce(allUpTicket, leg);
                            scenarioGenerator.Generate(mutator, sourcer, pnLConsumer, ticket);
                        }
                    }
                }
                watch.Stop();
                Console.WriteLine("Time Elapsed in ms for Generating the single tickets of match number " + matchPlayed + " :  " + watch.ElapsedMilliseconds);
                watch.Restart();
                scenarioGenerator.Pick(mutator, sourcer, pnLConsumer, matchResults[matchPlayed]);
                reducer.Advance(allUpTickets, matchResults[matchPlayed]);
                matchPlayed++;
                watch.Stop();
                Console.WriteLine("Time Elapsed in ms for Resolving the single tickets of match number " +  matchPlayed +" :  " + watch.ElapsedMilliseconds);

            }

            //6. Query Dividend
            pnLConsumer.PrintAllInvestment();
            pnLConsumer.PrintAllDividend();
            pnLConsumer.PrintAllRefund();


            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " High Water Mark Emitted : " + mutator.HighWaterMark);
            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " High Water Mark Processed : " + pnLConsumer.GetHighWaterMark());
            Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Remaining All Up Tickets Unprocessed : " + allUpTickets.Count);

            return pnLConsumer.GetPnL();

        }
    }
}
