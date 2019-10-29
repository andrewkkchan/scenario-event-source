using OIData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OIFinancialUpdateEventSource.Consumer
{
    public class PnLInMemoryConsumer : IPnLConsumer
    {
        public readonly IDictionary<int, Result> matchResult = new Dictionary<int, Result>();
        public readonly IDictionary<int, IDictionary<Result, PnLState>> pnlStateScenario = new Dictionary<int, IDictionary<Result, PnLState>>();
        public long highWaterMark;

        public void Consume(Mutation mutation)
        {
            if (mutation.MutationType == MutationType.Ticket)
            {
                if (!pnlStateScenario.ContainsKey(mutation.PoolId))
                {
                    pnlStateScenario.Add(mutation.PoolId, new Dictionary<Result, PnLState>
                    {
                        {Result.WIN, new PnLState()},
                        {Result.LOSS, new PnLState()},
                        {Result.DRAW, new PnLState()},
                        {Result.REFUND, new PnLState()},
                    });
                }
                pnlStateScenario[mutation.PoolId][mutation.Result].AggregatedInvestment += mutation.Investment;
                pnlStateScenario[mutation.PoolId][mutation.Result].AggregatedDividend += mutation.Dividend;
                pnlStateScenario[mutation.PoolId][mutation.Result].AggregatedRefund += mutation.Refund;
            }

            if (mutation.MutationType == MutationType.Match)
            {
                matchResult.Add(mutation.PoolId, mutation.Result);
            }

            SetHighWaterMark(GetHighWaterMark() + 1);
        }



        public decimal QueryDividend(int poolId)
        {
            return pnlStateScenario[poolId][matchResult[poolId]].AggregatedDividend;
        }

        public void PrintAllDividend()
        {
            foreach (KeyValuePair<int, IDictionary<Result, PnLState>> pool in pnlStateScenario)
            {
                Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Aggregatged Dividend for pool " + pool.Key + " : " + QueryDividend(pool.Key));
            }
        }

        public long GetHighWaterMark()
        {
            return highWaterMark;
        }

        public void SetHighWaterMark(long value)
        {
            highWaterMark = value;
        }

        public decimal QueryRefund(int poolId)
        {
            return pnlStateScenario[poolId][matchResult[poolId]].AggregatedRefund;
        }

        public void PrintAllRefund()
        {
            foreach (KeyValuePair<int, IDictionary<Result, PnLState>> pool in pnlStateScenario)
            {
                Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Aggregatged Refund for pool " + pool.Key + " : " + QueryRefund(pool.Key));
            }
        }

        public decimal QueryInvestment(int poolId)
        {
            return pnlStateScenario[poolId][matchResult[poolId]].AggregatedInvestment;
        }

        public void PrintAllInvestment()
        {
            foreach (KeyValuePair<int, IDictionary<Result, PnLState>> pool in pnlStateScenario)
            {
                Console.WriteLine("At Thread " + Thread.CurrentThread.ManagedThreadId + " Aggregatged Investment for pool " + pool.Key + " : " + QueryInvestment(pool.Key));
            }
        }

        public IDictionary<int,PnLState> GetPnL()
        {
            IDictionary<int, PnLState> pnl = new Dictionary<int, PnLState>();
            foreach (KeyValuePair<int, IDictionary<Result, PnLState>> pool in pnlStateScenario)
            {
                pnl.Add(pool.Key, pnlStateScenario[pool.Key][matchResult[pool.Key]]);
            }
            return pnl;
        }
    }
}
