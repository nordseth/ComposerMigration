using EPiServer.Core.Transfer;
using EPiServer.Enterprise;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace EPiServer.ComposerMigration
{
    /// <summary>
    /// This class mirrors some of the standard import functionality to provide
    /// an opportunity to scan through the content that is to be imported.
    /// </summary>
    public class ImportPackageScanner
    {
        private const string PackageContentName = "/epix.xml";
        //private static ILog _log = LogManager.GetLogger(typeof(ImportPackageScanner));

        public event EventHandler<ContentScanEventArgs> ContentScanned;

        private IPackageReaderContext _context;

        public void ReadPackage(Stream stream, IPackageReaderContext context)
        {
            if (context == null || stream == null)
            {
                return;
            }

            try
            {
                using (var package = (ZipPackage)ZipPackage.Open(stream))
                {
                    ImportPartOfPackage(package, PackageContentName);
                }
            }
            catch (Exception ex)
            {
                context.Abort();
                if (!context.ContinueOnError)
                {
                    throw;
                }
                //Importer.Log.Error(string.Format("Failed to scan packet. Reason is {0}", ex.Message), ex);
            }
        }

        private void ImportPartOfPackage(ZipPackage package, string partName)
        {
            Uri partUri = new Uri(partName, UriKind.Relative);
            if (package.PartExists(partUri))
            {
                var part = (ZipPackagePart)package.GetPart(partUri);
                var stream = part.GetStream();
                if (stream.Length > 0)
                {
                    using (var reader = new XmlTextReader(stream))
                    {
                        ImportStream(package, reader);
                    }
                }
            }
        }

        private void ImportStream(ZipPackage package, XmlTextReader reader)
        {
            reader.ReadStartElement();

            while (true)
            {
                try
                {
                    if (_context.IsAborting && !_context.ContinueOnError)
                    {
                        break;
                    }
                    else if (reader.IsStartElement("pages"))
                    {
                        ImportContents<TransferPageData>(reader);
                    }
                    else if (!reader.Read())
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    _context.Abort();
                    if (!_context.ContinueOnError)
                    {
                        throw;
                    }
                    //Importer.Log.Error("Exception: {0}[{1}]", e, e.Message, e.InnerException != null ? e.InnerException.Message : String.Empty);
                }
            }
        }

        private void ImportContents<T>(XmlTextReader reader) where T : ITransferContentData
        {
            reader.Read(); // Read away 
            while (reader.IsStartElement(typeof(T).Name) && (!_context.IsAborting || _context.ContinueOnError))
            {
                try
                {
                    XmlSerializer xml = new XmlSerializer(typeof(T));
                    ITransferContentData content = xml.Deserialize(reader) as ITransferContentData;
                    if (content.RawContentData == null)
                    {
                        continue;
                    }

                    OnContentScanned(new ContentScanEventArgs(content, null /*_context.Importer*/));
                }
                catch (Exception e)
                {
                    _context.Abort();
                    if (!_context.ContinueOnError)
                    {
                        throw;
                    }
                    //Importer.Log.Error(String.Format("Failed to Import Content. Reason is {0}", e.Message), e);
                }
            }

            reader.ReadEndElement(); // Contents
        }


        protected virtual void OnContentScanned(ContentScanEventArgs e)
        {
            var evt = ContentScanned;
            if (evt != null)
            {
                evt(this, e);
            }
        }
    }

}
