using OIData;
using System;
using System.Collections.Generic;
using System.Text;

namespace OIFinancialUpdateEventSource.Processor
{
    public interface IBetProcessor
    {
        IDictionary<int,PnLState> Process(int poolSize);
    }

}
