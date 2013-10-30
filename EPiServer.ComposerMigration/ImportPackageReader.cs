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
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using log4net;

namespace EPiServer.ComposerMigration
{
    public class ImportPackageReader
    {
        private const string PackageContentName = "/epix.xml";
		private static readonly ILog Logger = LogManager.GetLogger(typeof(ImportPackageReader));
		private readonly IEnumerable<IXmlElementParser> _elementParsers;
        private readonly IPackageReaderContext _readerContext;

        public ImportPackageReader(IEnumerable<IXmlElementParser> elementParsers, IPackageReaderContext readerContext)
        {
            _elementParsers = elementParsers;
            _readerContext = readerContext ?? NullPackageReaderContext.Instance;
        }

        public virtual void ReadPackage(Stream stream)
        {
            if (stream == null)
                return;

            try
            {
                using (var package = (ZipPackage)ZipPackage.Open(stream))
                {
                    ReadPartOfPackage(package, PackageContentName, ReadXmlStream);
                }
            }
            catch (Exception ex)
            {
                _readerContext.Abort();
                if (!_readerContext.ContinueOnError)
                {
                    throw;
                }
                Logger.Error(string.Format("Failed to read packet. Reason is {0}", ex.Message));
            }

        }

        public virtual void ReadXmlStream(XmlReader reader)
        {
            reader.Read();

            int version = -1;
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "version":
                        int.TryParse(reader.Value, out version);
                        break;
                }
            }

            Logger.DebugFormat("Reading content package with version {0}.", version);

            while (reader.Read())
            {
                try
                {
                    if (_readerContext.IsAborting && !_readerContext.ContinueOnError)
                    {
                        break;
                    }

                    foreach (var transferModule in _elementParsers)
                    {
                        if (transferModule.SupportsCurrentElement(reader, version))
                        {
                            transferModule.ParseElement(reader);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _readerContext.Abort();
                    if (!_readerContext.ContinueOnError)
                    {
                        throw;
                    }
                    Logger.ErrorFormat("Exception: {0}[{1}]", ex, ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
                }
            }
        }

        private static void ReadPartOfPackage(ZipPackage package, string partName, Action<XmlReader> partsReader)
        {
            Uri partUri = new Uri(partName, UriKind.Relative);
            if (!package.PartExists(partUri))
            {
                return;
            }
            var part = (ZipPackagePart)package.GetPart(partUri);
            var stream = part.GetStream();
            if (stream.Length == 0)
            {
                return;

            }

            using (var reader = new XmlTextReader(stream))
            {
                partsReader(reader);
            }
        }
    }
}
