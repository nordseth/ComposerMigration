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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using log4net;

namespace EPiServer.ComposerMigration.CodeGeneration
{
    public class PageTypeElementParser : IXmlElementParser
    {
        private const int MaxVersionSupported = 2;
		private static readonly ILog Logger = LogManager.GetLogger(typeof(PageTypeElementParser));
		private static readonly HashSet<string> IgnoredPageTypes = new HashSet<string>(new[] { 
            "SysRoot", 
            "SysRecycleBin",
            ComposerPageTypes.Prefix + ComposerPageTypes.GlobalFunctionEditPage,
            ComposerPageTypes.Prefix + ComposerPageTypes.Container,
            ComposerPageTypes.Prefix + ComposerPageTypes.PersonalizationContainer
        }, StringComparer.OrdinalIgnoreCase);

        private readonly IContentTypeMapper _mapper;
        private readonly IContentTypeWriter _contentTypeWriter;
        private readonly IPackageReaderContext _readerContext;

        public PageTypeElementParser(IContentTypeMapper mapper, IContentTypeWriter contentTypeWriter, IPackageReaderContext readerContext)
        {
            _mapper = mapper;
            _contentTypeWriter = contentTypeWriter;
            _readerContext = readerContext ?? NullPackageReaderContext.Instance;
        }

        public virtual bool SupportsCurrentElement(XmlReader reader, int version)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return version <= MaxVersionSupported && reader.IsStartElement("pagetypes");
        }

        public virtual void ParseElement(XmlReader reader)
        {
            var pageTypes = Deserialize(reader);
            foreach (var pageType in pageTypes.Where(pt => !IgnoredPageTypes.Contains(pt.Name)))
            {
                if (_readerContext.IsAborting)
                {
                    break;
                }
                try
                {
                    var contentType = _mapper.Map(pageType);
                    _contentTypeWriter.Write(contentType);
                }
                catch (Exception ex)
                {
                    _readerContext.Abort();
                    if (!_readerContext.ContinueOnError)
                    {
                        throw;
                    }
                    Logger.Error("Failed to parse xml content.", ex);
                }
            }
        }

        public virtual IEnumerable<PageType> Deserialize(XmlReader reader)
        {
            var serializer = new XmlSerializer(typeof(PageType));

            while (!reader.EOF)
            {
                if (reader.IsStartElement("PageType"))
                {
                    Logger.DebugFormat("Deserializing node: '{0}'", reader.Name);
                    yield return serializer.Deserialize(reader) as PageType;
                }
                else
                {
                    reader.Read();
                }
            }
        }

    }
}
