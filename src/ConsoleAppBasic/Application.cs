using ConsoleAppBasic.Services;
using ConsoleAppBasic.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppBasic.App
{
    public class Application
    {
        private ILogger _logger;
        private ISomeService _someservice;

        public Application(ILogger<Application> logger, ISomeService someservice)
        {
            _logger = logger;
            _someservice = someservice;
        }

        public async Task Run()
        {
            try
            {
                _logger.LogInformation(LoggingEvents.GENERAL_INFO, "In Application Run");
            }          
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
         }
    }
}
