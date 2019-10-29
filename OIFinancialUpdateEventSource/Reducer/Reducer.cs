using OIData;
using System;
using System.Collections.Generic;
using System.Text;

namespace OIFinancialUpdateEventSource.Reducer
{
    public interface IReducer
    {
        SingleTicket Reduce(AllUpTicket allUpTicket, Leg leg);
        void Advance(List<AllUpTicket> allUpTickets, MatchResult matchResult);
    }
}
