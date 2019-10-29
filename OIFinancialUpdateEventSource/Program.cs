using OIData;
using OIFinancialUpdateEventSource.Consumer;
using OIFinancialUpdateEventSource.EventReceiver;
using OIFinancialUpdateEventSource.Generator;
using OIFinancialUpdateEventSource.Mutator;
using OIFinancialUpdateEventSource.Processor;
using OIFinancialUpdateEventSource.Reducer;
using OIFinancialUpdateEventSource.Sourcer;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace OIFinancialUpdateEventSource
{
    class Program
    {
#pragma warning disable IDE0060 // Remove unused parameter
        static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            Task<IDictionary<int, PnLState>> task1 = Task<IDictionary<int, PnLState>>.Factory.StartNew(() =>
            {
                IUnityContainer memoryContainer = new UnityContainer();
                memoryContainer.RegisterType<IBetProcessor, AllUpProcessor>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<IMutator, InMemoryMutator>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<IPnLConsumer, PnLInMemoryConsumer>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<ISourcer, InMemorySourcer>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<IReducer, AllUpReducer>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<IScenarioGenerator, ScenarioGenerator>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<IEventReceiver, AllUpEventReceiver>(TypeLifetime.Singleton);
                Console.WriteLine("In memory processor is processing on Thread " + Thread.CurrentThread.ManagedThreadId);
                return memoryContainer.Resolve<IBetProcessor>().Process(2000000);
            });

            Task<IDictionary<int, PnLState>> task2 = Task<IDictionary<int, PnLState>>.Factory.StartNew(() =>
            {
                IUnityContainer memoryContainer = new UnityContainer();
                memoryContainer.RegisterType<IBetProcessor, AllUpProcessor>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<IMutator, InMemoryMutator>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<IPnLConsumer, PnLInMemoryConsumer>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<ISourcer, InMemorySourcer>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<IReducer, AllUpReducer>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<IScenarioGenerator, ScenarioGenerator>(TypeLifetime.Singleton);
                memoryContainer.RegisterType<IEventReceiver, AllUpChangeEventReceiver>(TypeLifetime.Singleton);
                Console.WriteLine("In memory processor is processing on Thread " + Thread.CurrentThread.ManagedThreadId);
                return memoryContainer.Resolve<IBetProcessor>().Process(2000000);
            });
            Task.WaitAll(task1, task2);
            foreach (KeyValuePair<int, PnLState> pool in task1.Result)
            {
                PnLState originalState = task1.Result[pool.Key];
                PnLState targetState = task2.Result[pool.Key];
                Console.WriteLine("******Reversal for Pool " + pool.Key);
                Console.WriteLine("Reverse Dividend : " + (targetState.AggregatedDividend - originalState.AggregatedDividend));
                Console.WriteLine("Reverse Refund : " + (targetState.AggregatedRefund - originalState.AggregatedRefund));
                Console.WriteLine("Reverse Investment : " + (targetState.AggregatedInvestment - originalState.AggregatedInvestment));
            }

            Console.WriteLine("All threads complete");

        }
    }
}
