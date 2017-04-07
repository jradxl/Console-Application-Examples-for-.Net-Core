using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace ConsoleUserSecrets
{
    public class Program
    {

        /*   These are added to ConsoleUserSecrets.csproj
         *   
         *   <PropertyGroup>
         *     <UserSecretsId>John-ConsoleUserSecrets</UserSecretsId>
         *   </PropertyGroup>
         * 
         *   <ItemGroup>
         *     <DotNetCliToolReference Include="Microsoft.Extensions.SecretManager.Tools" Version="1.0.0-msbuild3-final" />
         *   </ItemGroup>
         * 
         *   Add this to project using Nuget
         *        Microsoft.Extensions.Configuration
         *        Microsoft.Extensions.Configuration.UserSecrets
         *        Microsoft.Extensions.Configuration.Binder
         *        and others....
         *        
         *   From a Command Prompt
         *     dotnet user-secrets set MyKey1 MyValue1
         *     
         *   In a Console App, there is no "Manage User Secrets" on a right-click of the project. Bhah!
         */

        static void Main(string[] args)
        {
            Console.WriteLine("Console User Secrets And Configuration with DI");

            var builder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                //Set Property to Copy Always
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>();
                
            var configuration = builder.Build();

            foreach (var i in configuration.AsEnumerable())
            {
                var mysecretKey = i.Key;
                var mysecretValue = i.Value;
                Console.WriteLine("Method1: The following were found: {0} = {1}", mysecretKey, mysecretValue);
            }

            var xx = configuration.AsEnumerable().Where<KeyValuePair<string,string>>(x => x.Key == "wizards:1:Age");
            foreach (var i in xx)
            {
                Console.WriteLine("Method2: The following were found: {0} = {1}", i.Key, i.Value);
            }

            var yy = configuration["wizards:1:Age"];
            Console.WriteLine("Method3: The following value was found: {0}", yy);

            IConfigurationSection zz = configuration.GetSection("wizards:1:Age");
            Console.WriteLine("Method3: The following values found: Path {0}, Key {1}, Value {2}", zz.Path, zz.Key, zz.Value);

            var aa = configuration.GetSection("wizards").GetChildren();
            foreach (var i in aa)
            {
                Console.WriteLine("Method4: The following were found: {0}, {1}, {2}", i.Path, i.Key, i.Value);
                var nn = i.GetChildren();
                foreach (var p in nn)
                {
                   Console.WriteLine("Method4a: The following were found: {0}, {1}, {2}", p.Path, p.Key, p.Value);
                }
            }

            //Looks for a "ConnectionStrings" key
            var bb = configuration.GetConnectionString("SQLServer");
            Console.WriteLine("Method5: SQLServer connection string is: {0}", bb);

            //Creates the Instance and provides value(s). Value already in Class takes precedence if present.
            //This is quite useful! No use of DI here yet!
            //I'm sure the GetSection is needed, and the dual key a surprise.
            //I feel this is the most effective way of getting complex data back from the JSON file without DI and IOptions.
            var cc = configuration.GetSection("SomeAppConfig:AppConfiguration").Get<AppConfiguration>();
            Console.WriteLine("Method6: Get Value from a Class: {0}, {1}", cc.SomeValue, cc.SomeOtherValue);

            //Will populate the Instance from values in appsettings[.?].json
            //This is quite useful! No use of DI here yet!
            var appOptions = new AppOptions();
            var dd = configuration.GetSection("App");
            dd.Bind(appOptions); //Probably a bug as it returns void.
            Console.WriteLine("Method7: Get Value from an recursively populated Instance: {0}, {1}, {2}", appOptions.Connection.Value, appOptions.Profile.Machine, appOptions.Window.Height);


            Console.WriteLine("Press Enter to Quit.");
            Console.ReadLine();
        }
    }

    public class AppConfiguration
    {
        public AppConfiguration ()
        {
            SomeValue = "1234567";
        }

        public string SomeValue { get; set; }
        public string SomeOtherValue { get; set; }
    }

    public class AppOptions
    {
        public Window Window { get; set; }
        public Connection Connection { get; set; }
        public Profile Profile { get; set; }
    }

    public class Window
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class Connection
    {
        public string Value { get; set; }
    }

    public class Profile
    {
        public string Machine { get; set; }
    }
}
