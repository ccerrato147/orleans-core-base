using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using interfaces;

namespace grains
{
    public class SimpleTextGrain : Orleans.Grain, ISimpleText
    {
        private readonly ILogger logger;
        public SimpleTextGrain(ILogger<SimpleTextGrain> logger)
        {
            this.logger = logger;
        }
        public Task<string> GetString(string param)
        {
            logger.LogInformation("The grain has received a parameter");
            return Task.FromResult($@"The simple text is: {param}");
        }
    }
}