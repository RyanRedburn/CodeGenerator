using CodeGenerator.Enumeration;
using CodeGenerator.Generator;
using CodeGenerator.Interface.Repository;
using CodeGenerator.Interface.Service;
using CodeGenerator.Repository;
using CodeGenerator.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeGenerator.Command
{
    internal class Program
    {
        private static readonly Dictionary<GeneratedFileType, string> _fileDirectories = new Dictionary<GeneratedFileType, string>
            {
                { GeneratedFileType.CSharpModel, "C#" },
                { GeneratedFileType.TSqlQuery, "T-SQL" }
            };

        private static void Main(string[] args)
        {
            try
            {
                // Get the app config, request input from the user as appropriate, and create relevant directories if they don't exist.
                var appConfig = GetConfiguration();

                if (appConfig.RequestFullConnectionOnExec || appConfig.RequestCatalogOnlyOnExec || string.IsNullOrWhiteSpace(appConfig.SourceConnectionString))
                    appConfig.SourceConnectionString = RequestConnectionString(appConfig.RequestFullConnectionOnExec ? false : appConfig.RequestCatalogOnlyOnExec, appConfig.SourceConnectionString);

                if (appConfig.CSharpModelConfiguration.Active && appConfig.CSharpModelConfiguration.RequestNameSpaceOnExec)
                    appConfig.CSharpModelConfiguration.ModelNameSpace = RequestUserInput("C# model file name space:", true);

                if (appConfig.RequestCodeDirectoryOnExec)
                    appConfig.CodeDirectory = RequestUserInput("Code generation output directory:", true);
                if (!Directory.Exists(appConfig.CodeDirectory))
                    Directory.CreateDirectory(appConfig.CodeDirectory);

                // Get the file generation service and add all active generators.
                var fgService = GetService(appConfig);

                AddFileGenerators(ref fgService, appConfig);

                // Generate code files and write them to disk.
                if (fgService.FileGenerators.Count() > 0)
                {
                    var fileDictionary = fgService.GenerateFiles();

                    foreach (var entry in fileDictionary)
                    {
                        var fileLocation = new DirectoryInfo(appConfig.CodeDirectory + _fileDirectories[entry.Key] + "\\");
                        if (!fileLocation.Exists)
                            fileLocation.Create();

                        foreach (var file in entry.Value)
                        {
                            File.WriteAllText(fileLocation + file.FileName, file.FileContents);
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine("Code generation completed. Output was written to " + appConfig.CodeDirectory);
                }
                else
                    Console.WriteLine("No active file generators detected.");
            }
            catch (TargetInvocationException e)
            {
                WriteConsoleError(e.InnerException.Message);
            }
            catch (Exception e)
            {
                WriteConsoleError(e.Message);
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Simple wrapper for writing an error message to the console.
        /// </summary>
        /// <param name="message">Error message.</param>
        private static void WriteConsoleError(string message)
        {
            Console.WriteLine("Code generation failed with error:");
            Console.WriteLine(message);
        }

        /// <summary>
        /// Prompts a user for input using the given message.
        /// </summary>
        /// <param name="prompt">The message to use as a prompt.</param>
        /// <param name="retryOnFailure">Whether or not the user should continue to be prompted on submitting invalid input.</param>
        /// <returns>User string input.</returns>
        private static string RequestUserInput(string prompt, bool retryOnFailure = false)
        {
            string input = null;
            var inputIsValid = false;
            do
            {
                Console.WriteLine(prompt);
                input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                    inputIsValid = true;
            } while (retryOnFailure && !inputIsValid);
            return input;
        }

        /// <summary>
        /// Gets the configuration settings from the appsettings file.
        /// </summary>
        /// <returns>ApplicationConfiguration with settings from file.</returns>
        private static ApplicationConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var appConfig = config.GetSection("ApplicationConfiguration").Get<ApplicationConfiguration>();

            return appConfig;
        }

        /// <summary>
        /// Prompt the user for connection string information.
        /// </summary>
        /// <param name="catalogOnly">If true, only the database name is requested.</param>
        /// <param name="connString">If catalogOnly, the base connection string is required so it can be modified.</param>
        /// <returns>Database connection string.</returns>
        private static string RequestConnectionString(bool catalogOnly = false, string connString = null)
        {
            if (catalogOnly && !string.IsNullOrEmpty(connString))
            {
                var catalog = RequestUserInput("Source database:", true);

                var result = new StringBuilder();
                int position = 0, equalCount = 0, semicolonCount = 0;
                var building = true;
                while (building)
                {
                    if (connString[position] == '=')
                    {
                        equalCount++;
                        if (equalCount == 2)
                        {
                            result.Append(connString.Substring(0, position + 1));
                            result.Append(catalog);
                        }
                    }
                    else if (connString[position] == ';')
                    {
                        semicolonCount++;
                        if (semicolonCount == 2)
                        {
                            result.Append(connString.Substring(position));
                            building = false;
                        }
                    }
                    position++;
                }

                return result.ToString();
            }

            connString = "data source={0};initial catalog={1};user id={2};password={3};";

            var dbServer = RequestUserInput("Source database server:", true);
            var database = RequestUserInput("Source database:", true);
            var userName = RequestUserInput("Database server user name:", true);
            var password = RequestUserInput("Database server password for user:", true);

            return string.Format(connString, dbServer, database, userName, password);
        }

        /// <summary>
        /// Gets an instance of a file generation service using the given configuration.
        /// </summary>
        /// <param name="appConfig">Configuration object to take settings from.</param>
        /// <returns>Configured file generation service.</returns>
        private static IFileGenerationService GetService(ApplicationConfiguration appConfig)
        {
            var services = new ServiceCollection()
                .AddLogging(x => x.AddConsole())
                .AddTransient<IFileGenerationService, FileGenerationService>()
                .AddTransient(typeof(ILogger), x => x.GetService<ILoggerFactory>().CreateLogger("CodeGenerator"));

            switch (appConfig.ConnectionType)
            {
                case ConnectionType.SqlServer:
                    services
                        .AddTransient<ISpecificationRepository, SqlServerSpecificationRepository>()
                        .AddTransient(typeof(IDbConnection), x => new SqlConnection(appConfig.SourceConnectionString));
                    break;
            }

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetService<IFileGenerationService>();
        }

        /// <summary>
        /// Adds all active file generators to the given service using the given configuration.
        /// </summary>
        /// <param name="fgService">Target file generation service.</param>
        /// <param name="appConfig">Configuration object to take settings from.</param>
        private static void AddFileGenerators(ref IFileGenerationService fgService, ApplicationConfiguration appConfig)
        {
            if (appConfig.CSharpModelConfiguration.Active)
            {
                var csharpGenerator = new CSharpModelFileGenerator
                {
                    ModelNameSpace = appConfig.CSharpModelConfiguration.ModelNameSpace,
                    AddAnnotations = appConfig.CSharpModelConfiguration.AddAnnotations,
                    OnlyExactMatchForAnnotations = appConfig.CSharpModelConfiguration.OnlyExactMatchForAnnonations
                };
                fgService.AddFileGenerator(csharpGenerator);
            }

            if (appConfig.TSqlQueryConfiguration.Active)
            {
                var tsqlGenerator = new TSqlQueryFileGenerator() { QuoteIdentifiers = appConfig.TSqlQueryConfiguration.QuoteIdentifiers };
                fgService.AddFileGenerator(tsqlGenerator);
            }
        }
    }
}
