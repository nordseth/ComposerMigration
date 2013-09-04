using EPiServer.ComposerMigration.DataAbstraction;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace EPiServer.ComposerMigration.CodeGeneration.Test
{
    [TestClass]
    public class PageTypeElementParserTests
    {
        private const int OverlookPageTypeCount = 44;

        private PageTypeElementParser CreateSubject()
        {
            return new PageTypeElementParser(new Mock<IContentTypeMapper>().Object, new Mock<IContentTypeWriter>().Object, null);
        }

        [TestMethod]
        public void SupportsCurrentElement_WithPageTypeRootElement_ShouldReturnTrue()
        {
            var subject = CreateSubject();
            using (var xmlReader = CreateAndPositionReader("<pagetypes />"))
            {
                var result = subject.SupportsCurrentElement(xmlReader, 0);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void SupportsCurrentElement_WithFutureVersionsOfPageTypeElement_ShouldReturnFalse()
        {
            var subject = CreateSubject();
            using (var xmlReader = CreateAndPositionReader("<pagetypes />"))
            {
                var result = subject.SupportsCurrentElement(xmlReader, 3);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void SupportsCurrentElement_WithDifferentElement_ShouldReturnFalse()
        {
            var subject = CreateSubject();
            using (var xmlReader = CreateAndPositionReader("<contenttypes />"))
            {
                var result = subject.SupportsCurrentElement(xmlReader, 2);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        [DeploymentItem(@"EPiServer.ComposerMigration.Test\SimplifiedPageTypeExport.xml")]
        public void Deserialize_FromExportedXml_ShouldDeserializeToRawPageTypes()
        {
            var subject = CreateSubject();
            using (var xmlReader = CreateAndPositionReader("SimplifiedPageTypeExport.xml"))
            {
                var result = subject.Deserialize(xmlReader);
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count());
            }
        }

        [TestMethod]
        [DeploymentItem(@"EPiServer.ComposerMigration.Test\OverlookExport.xml")]
        public void Deserialize_FromExportedXml_ShouldDeserializeAllPageTypes()
        {
            var subject = CreateSubject();
            using (var xmlReader = CreateAndPositionReader("OverlookExport.xml"))
            {
                var result = subject.Deserialize(xmlReader);
                Assert.IsNotNull(result);
                Assert.AreEqual(OverlookPageTypeCount, result.Count());
            }
        }

        [TestMethod]
        [DeploymentItem(@"EPiServer.ComposerMigration.Test\SimplifiedPageTypeExport.xml")]
        public void Deserialize_FromExportedXml_ShouldHaveAllSimpleProperties()
        {
            var subject = CreateSubject();
            using (var xmlReader = CreateAndPositionReader("SimplifiedPageTypeExport.xml"))
            {
                var result = subject.Deserialize(xmlReader).First();

                Assert.AreEqual(new Guid("de67d4e6-2119-4c35-86d8-e68fdbd47467"), result.GUID);
                Assert.AreEqual("[Overlook] Standard page", result.Name);
                Assert.AreEqual("~/Templates/DemoSite/Pages/OrdinaryPage.aspx", result.ExportableFileName);
                Assert.AreEqual(true, result.IsAvailable);
                Assert.AreEqual(100, result.SortOrder);
                Assert.AreEqual(27, result.AllowedPageTypeNames.Count());
            }
        }

        [TestMethod]
        [DeploymentItem(@"EPiServer.ComposerMigration.Test\SimplifiedPageTypeExport.xml")]
        public void Deserialize_FromExportedXml_ShouldHaveACL()
        {
            var subject = CreateSubject();
            using (var xmlReader = CreateAndPositionReader("SimplifiedPageTypeExport.xml"))
            {
                var result = subject.Deserialize(xmlReader).First().ACL;

                Assert.IsNotNull(result.Entries);
                Assert.AreEqual(1, result.Entries.Count());
            }
        }

        [TestMethod]
        [DeploymentItem(@"EPiServer.ComposerMigration.Test\SimplifiedPageTypeExport.xml")]
        public void Deserialize_FromExportedXml_ShouldDeserializeAclCorrectly()
        {
            var subject = CreateSubject();
            using (var xmlReader = CreateAndPositionReader("SimplifiedPageTypeExport.xml"))
            {
                var result = subject.Deserialize(xmlReader).First().ACL.Entries.First();

                Assert.AreEqual("Everyone", result.Name);
                Assert.AreEqual(AccessLevel.Create, result.Access);
                Assert.AreEqual(SecurityEntityType.Role, result.EntityType);
            }
        }

        [TestMethod]
        [DeploymentItem(@"EPiServer.ComposerMigration.Test\SimplifiedPageTypeExport.xml")]
        public void Deserialize_FromExportedXml_ShouldHavePropertyDefinitions()
        {
            var subject = CreateSubject();
            using (var xmlReader = CreateAndPositionReader("SimplifiedPageTypeExport.xml"))
            {
                var result = subject.Deserialize(xmlReader).First().Definitions;

                Assert.IsNotNull(result);
                Assert.AreEqual(7, result.Count());
            }
        }

        [TestMethod]
        [DeploymentItem(@"EPiServer.ComposerMigration.Test\SimplifiedPageTypeExport.xml")]
        public void Deserialize_FromExportedXml_ShouldDeserializePropertyDefinitionsCorrectly()
        {
            var subject = CreateSubject();
            using (var xmlReader = CreateAndPositionReader("SimplifiedPageTypeExport.xml"))
            {
                var result = subject.Deserialize(xmlReader).First().Definitions.First();

                Assert.AreEqual("Heading", result.Name);
                Assert.AreEqual("Heading", result.EditCaption);
                Assert.AreEqual(false, result.Required);
                Assert.AreEqual(false, result.Searchable);
                Assert.AreEqual("String", result.Type.DataType);
                Assert.AreEqual("Information", result.Tab.Name);
                Assert.AreEqual(false, result.LanguageSpecific);
            }
        }

        private static XmlReader CreateAndPositionReader(string xml)
        {
            XmlReader reader;
            if (xml.StartsWith("<"))
            {
                reader = XmlTextReader.Create(new StringReader(xml));
            }
            else
            {
                reader = XmlTextReader.Create(File.OpenRead(xml));
                reader.ReadStartElement();
            }
            reader.MoveToContent();
            return reader;
        }

    }
}
