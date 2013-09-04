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
using EPiServer.ComposerMigration.DataAbstraction;
using EPiServer.Core.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace EPiServer.ComposerMigration
{
    public class ImportDataCollectorElementParser : IXmlElementParser
    {
        private const int MaxVersionSupported = 2;
        private static readonly Common.Logging.ILog Logger = Common.Logging.LogManager.GetCurrentClassLogger();

        private readonly IEnumerable<IImportDataCollector> _collectors;
        private readonly IPackageReaderContext _readerContext;

        public ImportDataCollectorElementParser(IEnumerable<IImportDataCollector> collectors, IPackageReaderContext readerContext)
        {
            _collectors = collectors ?? Enumerable.Empty<IImportDataCollector>();
            _readerContext = readerContext ?? NullPackageReaderContext.Instance;
        }

        public virtual bool SupportsCurrentElement(XmlReader reader, int version)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return version <= MaxVersionSupported && reader.IsStartElement("pages");
        }

        public virtual void ParseElement(XmlReader reader)
        {
            reader.Read(); // Read away 
            while (reader.IsStartElement(typeof(TransferPageData).Name) && (!_readerContext.IsAborting || _readerContext.ContinueOnError))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(TransferPageData));
                    var content = serializer.Deserialize(reader) as ITransferContentData;
                    if (content.RawContentData == null)
                    {
                        continue;
                    }

                    foreach (var collector in _collectors)
                    {
                        collector.Collect(content);
                    } 
                    //OnContentScanned(new ContentScanEventArgs(content, null /*_context.Importer*/));
                }
                catch (Exception ex)
                {
                    _readerContext.Abort();
                    if (!_readerContext.ContinueOnError)
                    {
                        throw;
                    }
                    Logger.Error(String.Format("Failed to Import Content. Reason is {0}", ex.Message), ex);
                }
            }

            reader.ReadEndElement(); // Contents
        }
    }
}
