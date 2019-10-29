using System.Collections.Generic;
using OIData;

namespace OIFinancialUpdateEventSource.Consumer
{
    public interface IPnLConsumer
    {
        long GetHighWaterMark();
        void SetHighWaterMark(long value);
        void Consume(Mutation mutation);
        decimal QueryDividend(int poolId);

        void PrintAllDividend();

        decimal QueryRefund(int poolId);

        void PrintAllRefund();
        decimal QueryInvestment(int poolId);
        void PrintAllInvestment();
        IDictionary<int, PnLState> GetPnL();
    }

}
