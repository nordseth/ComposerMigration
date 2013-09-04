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
using Common.Logging;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.Enterprise;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using StructureMap;
using System;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace EPiServer.ComposerMigration.Import
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ImportInitializationModule : IInitializableModule, IConfigurableModule
    {
        private static ILog Logger;
        private IContainer _container;
        private bool _enabled;

        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            _container = context.Container;
            InitalizeLogger();
            InitializeContainer(_container);
        }

        void IInitializableModule.Initialize(InitializationEngine context)
        {
            _enabled = ServiceLocator.Current.GetInstance<IComposerImportOptions>().Enabled;
            if (_enabled)
            {
                DataImporter.Importing += DataImporter_Importing;
                DataImporter.ContentImporting += DataImporter_ContentImporting;
            }
        }

        void IInitializableModule.Uninitialize(InitializationEngine context)
        {
            if (_enabled)
            {
                DataImporter.Importing -= DataImporter_Importing;
                DataImporter.ContentImporting -= DataImporter_ContentImporting;
            }
        }

        void IInitializableModule.Preload(string[] parameters) { }

        private void DataImporter_Importing(object sender, EventArgs e)
        {
            var dataImporter = (DataImporter)sender;

            IntializeContainerForImport(_container, dataImporter);

            var packageReader = ServiceLocator.Current.GetInstance<ImportPackageReader>();
            packageReader.ReadPackage(dataImporter.Stream);

            var contentMap = ServiceLocator.Current.GetInstance<IContentMap>();
            if (contentMap.PageCount == 0)
            {
                dataImporter.Log.Warning("The import was examined by the EPiServer.ComposerMigration.Import module, but the package contained no Composer pages. If your Composer migration is completed it's recommended that you uninstall the module.");
            }

            // TODO: Remove this! This is to keep myself sane while testing - as destination root is required
            if (ContentReference.IsNullOrEmpty(dataImporter.DestinationRoot) && !ContentReference.IsNullOrEmpty(ContentReference.StartPage))
            {
                dataImporter.DestinationRoot = ContentReference.StartPage;
            }
        }

        private void DataImporter_ContentImporting(DataImporter dataImporting, ContentImportingEventArgs e)
        {
            if (!RunContentTransforms(dataImporting, e.TransferContentData))
            {
                e.Cancel = true;
            };
        }

        protected virtual bool RunContentTransforms(DataImporter dataImporter, ITransferContentData content)
        {
            var transformers = ServiceLocator.Current.GetAllInstances<IImportTransform>();

            Logger.Trace("Running transforms on content");
            foreach (var transformer in transformers)
            {
                if (!transformer.Transform(content))
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual void IntializeContainerForImport(IContainer container, DataImporter dataImporter)
        {
            // Temporarily inject the current context (dataImporter) into the container
            var context = new DataImporterPackageReaderContext(dataImporter);
            container.Inject<IPackageReaderContext>(context);
            container.Inject<IContentTransferContext>(dataImporter);

            // Create a ContentMap object that will live over the current thread/request
            IContentMap contentMap = new ContentMap();
            container.Inject<IContentMap>(contentMap);
        }

        private static void InitalizeLogger()
        {
            var currentAdapter = LogManager.Adapter;
            if (LogManager.Adapter == null || LogManager.Adapter is Common.Logging.Simple.NoOpLoggerFactoryAdapter)
            {
                // set Adapter
                var properties = new NameValueCollection { { "configType", "EXTERNAL" } };
                LogManager.Adapter = new Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter(properties);
            }
            Logger = LogManager.GetCurrentClassLogger();
        }

        private static void InitializeContainer(IContainer container)
        {
            container.Configure(c =>
            {
                c.For<IImportTransform>().Add<IgnoredContentTransform>();
                c.For<IImportTransform>().Add<ContentAreaTransform>();
                c.For<IImportTransform>().Add<ComposerContainerTransform>();
                c.For<IImportTransform>().Add<TypeNameTransform>();
                c.For<IImportTransform>().Add<ComposerFunctionTransform>();
                c.For<IImportTransform>().Add<DynamicFunctionTransform>();
                c.For<IImportTransform>().Add<IgnoredPropertiesTransform>();

                c.For<IImportDataCollector>().Add<ContentMapCollector>();
                c.For<IImportDataCollector>().Add<PersonalizationContainerCollector>();

                c.For<IXmlElementParser>().Add<ImportDataCollectorElementParser>();

                // Read options from appSettings
                var options = new ConfigOptions(WebConfigurationManager.AppSettings);
                c.For<IComposerImportOptions>().Use(options);
                c.For<IComposerTranformationOptions>().Use(options);
            });
        }
    }

}
