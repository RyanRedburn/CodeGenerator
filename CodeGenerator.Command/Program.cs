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
                // Get the app config and create relevant directories if they don't exist.
                var appConfig = GetConfiguration();

                if (appConfig.CSharpModelConfiguration.Active && appConfig.CSharpModelConfiguration.RequestNameSpaceOnExec)
                {
                    Console.WriteLine("C# model file name space:");
                    appConfig.CSharpModelConfiguration.ModelNameSpace = Console.ReadLine();
                }

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

        private static void WriteConsoleError(string message)
        {
            Console.WriteLine("Code generation failed with error:");
            Console.WriteLine(message);
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

            if (string.IsNullOrWhiteSpace(appConfig.CodeDirectory))
                throw new InvalidOperationException("CodeDirectory most be a valid directory UNC path.");

            if (string.IsNullOrWhiteSpace(appConfig.SourceConnection))
                throw new InvalidOperationException("SourceConnection most be a valid connection string.");

            return appConfig;
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
                        .AddTransient(typeof(IDbConnection), x => new SqlConnection(appConfig.SourceConnection));
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
