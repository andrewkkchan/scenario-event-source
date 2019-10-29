using OIData;
using System;
using System.Collections.Generic;
using System.Text;

namespace OIFinancialUpdateEventSource.EventReceiver
{
    public interface IEventReceiver
    {

        void Receive(List<AllUpTicket> allupTickets, List<MatchResult> matchResults, int volume);
    }
    public class AllUpEventReceiver : IEventReceiver
    {
        public void Receive(List<AllUpTicket> allUpTickets, List<MatchResult> matchResults, int volume)
        {
            //A. Receive 1 million bets and win the pool
            for (int i = 1; i < volume/2 + 1; i++)
            {
                // 1. Flowing state in the form of all-up ticket bets coming in
                AllUpTicket ticket = new AllUpTicket
                {
                    TicketId = "17_2_1_" + (460100000 + i).ToString(),
                    BetId = 1,
                    Legs = new List<Leg>{
                        new Leg {
                            PoolId= 71000001,
                            CombinationId = 1,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000002,
                            CombinationId = 1,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000003,
                            CombinationId = 1,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000004,
                            CombinationId = 1,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000005,
                            CombinationId = 1,
                            Odds = 1.5m
                        }
                    },
                    UnitBets = 0.1m,
                    LegsPlayed = 1,
                };
                allUpTickets.Add(ticket);
            }

            for (int i = volume / 2 + 1; i < volume + 1; i++)
            {
                // 1. Flowing state in the form of all-up ticket bets coming in
                AllUpTicket ticket = new AllUpTicket
                {
                    TicketId = "17_2_1_" + (460100000 + i).ToString(),
                    BetId = 1,
                    Legs = new List<Leg>{
                        new Leg {
                            PoolId= 71000001,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000003,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000004,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000005,
                            Odds = 1.5m
                        },
                    },
                    UnitBets = 0.1m,
                    LegsPlayed = 1,
                };
                allUpTickets.Add(ticket);
            }
            matchResults.Add(new MatchResult
            {
                Result = Result.LOSS,
                PoolId = 71000002,
                CombinationId = 1
            });
            matchResults.Add(new MatchResult
            {
                Result = Result.DRAW,
                PoolId = 71000004,
                CombinationId = 1
            });
            matchResults.Add(new MatchResult
            {
                Result = Result.DRAW,
                PoolId = 71000001,
                CombinationId = 1
            });
            matchResults.Add(new MatchResult
            {
                Result = Result.WIN,
                PoolId = 71000005,
                CombinationId = 1
            });
            matchResults.Add(new MatchResult
            {
                Result = Result.WIN,
                PoolId = 71000003,
                CombinationId = 1
            });
        }
    }

    public class AllUpChangeEventReceiver : IEventReceiver
    {
        public void Receive(List<AllUpTicket> allUpTickets, List<MatchResult> matchResults, int volume)
        {
            //A. Receive 1 million bets and win the pool
            for (int i = 1; i < volume / 2 + 1; i++)
            {
                // 1. Flowing state in the form of all-up ticket bets coming in
                AllUpTicket ticket = new AllUpTicket
                {
                    TicketId = "17_2_1_" + (460100000 + i).ToString(),
                    BetId = 1,
                    Legs = new List<Leg>{
                        new Leg {
                            PoolId= 71000001,
                            CombinationId = 1,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000002,
                            CombinationId = 1,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000003,
                            CombinationId = 1,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000004,
                            CombinationId = 1,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000005,
                            CombinationId = 1,
                            Odds = 1.5m
                        }
                    },
                    UnitBets = 0.1m,
                    LegsPlayed = 1,
                };
                allUpTickets.Add(ticket);
            }

            for (int i = volume / 2 + 1; i < volume + 1; i++)
            {
                // 1. Flowing state in the form of all-up ticket bets coming in
                AllUpTicket ticket = new AllUpTicket
                {
                    TicketId = "17_2_1_" + (460100000 + i).ToString(),
                    BetId = 1,
                    Legs = new List<Leg>{
                        new Leg {
                            PoolId= 71000001,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000003,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000004,
                            Odds = 1.5m
                        },
                        new Leg{
                            PoolId=71000005,
                            Odds = 1.5m
                        },
                    },
                    UnitBets = 0.1m,
                    LegsPlayed = 1,
                };
                allUpTickets.Add(ticket);
            }
            matchResults.Add(new MatchResult
            {
                Result = Result.LOSS,
                PoolId = 71000002,
                CombinationId = 1
            });
            matchResults.Add(new MatchResult
            {
                Result = Result.WIN,
                PoolId = 71000004,
                CombinationId = 1
            });
            matchResults.Add(new MatchResult
            {
                Result = Result.DRAW,
                PoolId = 71000001,
                CombinationId = 1
            });
            matchResults.Add(new MatchResult
            {
                Result = Result.WIN,
                PoolId = 71000005,
                CombinationId = 1
            });
            matchResults.Add(new MatchResult
            {
                Result = Result.WIN,
                PoolId = 71000003,
                CombinationId = 1
            });
        }
    }
}
