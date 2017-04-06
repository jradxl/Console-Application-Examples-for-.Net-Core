using ConsoleAppBasic.App;
using ConsoleAppBasic.Configurations;
using ConsoleAppBasic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Linq;

namespace ConsoleAppBasic.Program
{
    //Dependency Injection to Program is not supported
    public class Program
    {
        private static string environment;

        //Switching AppSettings.Json files
        //Typically, appsettings.json, appsettings.production.json, appsettings.staging.json, appsettings.development.json
        //In this App, values are Production, Staging, and Development.
        //If not set, tne default is used.
        private const string defaultEnvironment = "Development";

        //Access point from OS.
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Console App, Basic. Program starting...");

            //Put an Enviroment Variable to the Key "CONSOLEAPP_BASIC_Environment" in the Project's Debug Tab.
            environment = Environment.GetEnvironmentVariable("CONSOLEAPP_BASIC_Environment");

            //CommandLine supersedes
            if (args != null && args.Count() == 1)
            {
                environment = args[0];
            }

            //Finally, if not yet set, then use the default
            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = defaultEnvironment;
            }

            //In ASP.NET, the Host calls this. Here we need to call manually 
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            //This is the Dependency Injection container
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            var app = serviceProvider.GetService<Application>();

            app.Run().Wait();

            Console.WriteLine("Press Enter to Quit.");
            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            //Add Configuration to the Container
            IConfigurationRoot configuration = GetConfiguration();
            services.AddSingleton<IConfiguration>(configuration);

            //Configure Seilog, to write logs to file
            Log.Logger = new LoggerConfiguration()
                              .Enrich.FromLogContext()
                              .WriteTo.LiterateConsole()
                              //.WriteTo.RollingFile("log-{Date}.txt")  //Option
                              //.WriteTo.RollingFile("log-{Date}.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}") //Option
                              .WriteTo.RollingFile(new JsonFormatter(), "log-{Date}.txt") //Option
                              .ReadFrom.Configuration(configuration)  //Option
                              .CreateLogger();

            Log.Information("LogFile Started - This will be written to the rolling file set");

            var loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug()
                .AddSerilog();

            //Support Logging
            services.AddSingleton(loggerFactory);
            services.AddLogging(); // Allow ILogger<T>

            //Support typed Options
            services.AddOptions();
            services.Configure<SomeConfiguration>(configuration.GetSection("SomeSection"));

            //Add Services
            services.AddSingleton<ISomeService, SomeService>();
            services.AddTransient<Application>();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            IConfigurationBuilder configuration = new ConfigurationBuilder()
                //This method is now Optional .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment.ToLowerInvariant()}.json", optional: true)
                //These EnvironmentVariables will override the appsettings.json and should be set like CONSOLEAPP-Basic:SomeValue
                //Order matters, so don't change the current order!
                //After adding a new Environment variable to the system, restart VS otherwise it won't be able to accessit.

                //Lets select the prefixes of the variables we want. Otherwise it will load ALL environment variables.
                .AddEnvironmentVariables(@"CONSOLEAPP-Basic_");

            //poor men's solution for the ASP.NET env.IsDevelopment()
            if (string.Compare(environment, defaultEnvironment, true) == 0)
            {
                //User secrets should only be used in development but console apps do not have an environment.
                //Avoid using Environment variables during development
                //http://asp.net-hacker.rocks/2016/07/11/user-secrets-in-aspnetcore.html
                configuration.AddUserSecrets(@"CONSOLEAPP-Basic");
            }
            return configuration.Build();
        }
    }
}
