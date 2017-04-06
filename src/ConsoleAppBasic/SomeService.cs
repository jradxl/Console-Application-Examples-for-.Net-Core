using ConsoleAppBasic.Events;
using Microsoft.Extensions.Logging;

namespace ConsoleAppBasic.Services
{
    public class SomeService  : ISomeService
    { 
        private ILogger _logger;

        public SomeService(ILogger<SomeService> logger)
        {
            _logger = logger;
            _logger.LogInformation(LoggingEvents.GENERAL_INFO, "In SomeService Constructor");
        }
    }

    public interface ISomeService
    {
    }
}
