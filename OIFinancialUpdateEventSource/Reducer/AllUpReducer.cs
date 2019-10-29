using OIData;
using System.Collections.Generic;

namespace OIFinancialUpdateEventSource.Reducer
{
    public class AllUpReducer: IReducer
    {
        public void Advance(List<AllUpTicket> allUpTickets, MatchResult matchResult)
        {
            foreach (AllUpTicket allUpTicket in allUpTickets)
            {
                foreach (Leg leg in allUpTicket.Legs) {
                    if (leg.PoolId == matchResult.PoolId)
                    {
                        if (matchResult.Result == Result.WIN)
                        {
                            allUpTicket.UnitBets *= allUpTicket.Legs[allUpTicket.LegsPlayed - 1].Odds;
                            allUpTicket.LegsPlayed++;
                        }
                        if (matchResult.Result == Result.LOSS)
                        {
                            allUpTicket.UnitBets = 0m;
                            allUpTicket.LegsPlayed++;
                        }
                        if (matchResult.Result == Result.REFUND)
                        {
                            allUpTicket.LegsPlayed++;
                        }
                        if (matchResult.Result == Result.DRAW)
                        {
                            allUpTicket.LegsPlayed++;
                        }
                    }
                }

            }
            allUpTickets.RemoveAll(item => (item.LegsPlayed > item.Legs.Count) || item.UnitBets == 0m);
        }


        public SingleTicket Reduce(AllUpTicket allUpTicket, Leg leg)
        {
            SingleTicket singleTicket =  new SingleTicket
            {
                TicketId = allUpTicket.TicketId,
                BetId = allUpTicket.BetId,
                Odds = leg.Odds,
                UnitBets = allUpTicket.UnitBets,
                PoolId = leg.PoolId,
                LegNo = allUpTicket.LegsPlayed
            };
            return singleTicket;
        }
    }
}
