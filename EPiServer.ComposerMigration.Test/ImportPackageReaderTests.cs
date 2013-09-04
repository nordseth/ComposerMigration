using EPiServer.ComposerMigration.CodeGeneration;
using EPiServer.ComposerMigration.DataAbstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Xml;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class ImportPackageReaderTests
    {
        private const int OverlookPageTypeCount = 44;

        [TestMethod]
        [DeploymentItem(@"EPiServer.ComposerMigration.Test\OverlookExport.xml")]
        public void ReadXmlStream_FromExportedXml_ShouldCallXmlTransferWithExactNumberOfPageTypes()
        {
            var xmlTransferMock = new Mock<IXmlElementParser>();
            int pageTypeCount = 0;
            xmlTransferMock.Setup(x => x.ParseElement(It.IsAny<XmlReader>())).Callback<XmlReader>(r => 
            {
                r.ReadToDescendant("PageType");
                do {
                    pageTypeCount++;
                }
                while (r.ReadToNextSibling("PageType"));
            });

            xmlTransferMock.Setup(x => x.SupportsCurrentElement(It.IsAny<XmlReader>(), It.IsAny<int>()))
                .Returns<XmlReader, int>((r, x) => r.IsStartElement("pagetypes"));

            var subject = new ImportPackageReader(new IXmlElementParser[] { xmlTransferMock.Object }, null);

            using (var xmlReader = CreateReader("OverlookExport.xml"))
            {
                subject.ReadXmlStream(xmlReader);
            }

            Assert.AreEqual(OverlookPageTypeCount, pageTypeCount);
        }

        [TestMethod]
        [DeploymentItem(@"EPiServer.ComposerMigration.Test\OverlookExport.xml")]
        public void ReadXmlStream_FromExportedXml_ShouldCallContentTypeRepositoryOnceForEachPageType()
        {
            var repository = new Mock<IContentTypeWriter>();
            var xmlTransfer = new PageTypeElementParser(new Mock<IContentTypeMapper>().Object, repository.Object, null);
            var subject = new ImportPackageReader(new IXmlElementParser[] { xmlTransfer }, null);

            using (var xmlReader = CreateReader("OverlookExport.xml"))
            {
                subject.ReadXmlStream(xmlReader);
            }

            repository.Verify(x => x.Write(It.IsAny<ContentType>()), Times.Exactly(OverlookPageTypeCount));
        }

        private static XmlReader CreateReader(string xml)
        {
            if (xml.StartsWith("<"))
            {
                return XmlTextReader.Create(new StringReader(xml));
            }

            return XmlTextReader.Create(File.OpenRead(xml));
        }

    }
}
