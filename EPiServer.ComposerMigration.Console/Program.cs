#region Copyright (C) 2013 EPiServer AB
/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this 
software and associated documentation files (the "Software"), to deal in the Software 
without restriction, including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/
#endregion
using CommandLine;
using CommandLine.Text;
using Common.Logging;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EPiServer.ComposerMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var options = new CommandLineOptions();

            if (Parser.Default.ParseArgumentsStrict(args, options) && ValidateOptions(options))
            {
                new Program().Run(options);
            }
        }

        private static bool ValidateOptions(CommandLineOptions options)
        {
            if (string.IsNullOrEmpty(options.Namespace))
            {
                Console.WriteLine("Please provide a valid namespace. It cannot be empty.");
                return false;
            }

            if (string.IsNullOrEmpty(options.SourcePackage) || !File.Exists(options.SourcePackage))
            {
                Console.WriteLine("Unable to find a source package at {0}.", options.SourcePackage);
                return false;
            }

            try
            {
                Directory.CreateDirectory(Path.Combine(options.OutputDirectory));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to locate or create the output directory [{0}].", ex.Message);
                return false;
            }

            // We won't bother with error messages here, just treat empty values as default.
            if (string.IsNullOrEmpty(options.PageBaseClass))
            {
                options.PageBaseClass = ComposerMigrationOptions.Default.PageBaseClass;
            }
            if (string.IsNullOrEmpty(options.BlockBaseClass))
            {
                options.PageBaseClass = ComposerMigrationOptions.Default.BlockBaseClass;
            }

            return true;
        }

        private void Run(CommandLineOptions options)
        {
            InitalizeLogger(options.Verbose ? LogLevel.Debug : LogLevel.Info);
            ConfigureContainer(options);
            var logger = LogManager.GetCurrentClassLogger();

            logger.InfoFormat("Generating classes from '{0}' and saving output to '{1}'", options.SourcePackage, options.OutputDirectory);

            var packageReader = ObjectFactory.GetInstance<ImportPackageReader>();
            Generate(packageReader, options.SourcePackage, options.OutputDirectory);
        }

        private void Generate(ImportPackageReader packageReader, string sourcePath, string destinationDirectory)
        {
            using (var fileStream = File.OpenRead(sourcePath))
            {
                packageReader.ReadPackage(fileStream);
            }
        }

        private static void ConfigureContainer(CommandLineOptions options)
        {
            ContainerBootstrapper.Bootstrap();
            ObjectFactory.Inject<IComposerTranformationOptions>(options);
            ObjectFactory.Inject<ICodeGenerationOptions>(options);
        }

        private static void InitalizeLogger(LogLevel logLevel)
        {
            if (!(LogManager.Adapter is Common.Logging.Simple.NoOpLoggerFactoryAdapter))
            {
                return;
            }

            var adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter
            {
                Level = logLevel,
                ShowDateTime = false,
                ShowLevel = false,
                ShowLogName = false
            };

            // set Adapter
            LogManager.Adapter = adapter;
        }

    }
}
