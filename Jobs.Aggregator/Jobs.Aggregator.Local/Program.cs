using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Jobs.Aggregator.Core;

namespace Jobs.Aggregator.Local
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var aggregatorService = Bootstrapper.Init();
            await aggregatorService.Aggregate();
        }
    }
}