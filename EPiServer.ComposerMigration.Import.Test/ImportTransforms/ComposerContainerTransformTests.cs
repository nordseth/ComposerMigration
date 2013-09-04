using EPiServer.ComposerMigration.DataAbstraction;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.DataAbstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class ComposerContainerTransformTests
    {
        [TestMethod]
        public void Transform_WhenContentItemIsNotAContainer_ShouldReturnTrue()
        {
            // Arrange
            var subject = CreateSubject(null, null);

            var content = new RawContent { Property = new[] { new RawProperty { Name = ComposerProperties.ContentFunction } } };

            // Act
            var result = subject.Transform(TestUtil.TransferData(content));

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Transform_WhenContentItemIsNotInContentMap_ShouldReturnFalse()
        {
            // Arrange
            var contentGuid = Guid.NewGuid();
            var contentMap = new Mock<IContentMap>();
            contentMap.Setup(x => x.GetParentPage(contentGuid)).Returns<IComposerPage>(null);
            var subject = CreateSubject(contentMap.Object, null);

            var content = new RawContent
            {
                Property = new[] { new RawProperty { Name = ComposerProperties.ContainerPage } }
            };

            // Act
            var result = subject.Transform(TestUtil.TransferData(content));

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Transform_WhenContainerHasMultipleLanguages_ShouldCallTransformContainerForAllLanguages()
        {
            // Arrange
            var parent = new ComposerPage { Name = "Parent name" };

            var contentMap = new Mock<IContentMap>();
            contentMap.Setup(x => x.GetParentPage(It.IsAny<Guid>())).Returns(parent);

            var subject = CreateSubject(contentMap.Object, null);

            var content = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.ContainerPage },
                    new RawProperty { Name = MetaDataProperties.PageName, Value = "Initial" }
                }
            };

            var language1 = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.ContainerPage },
                    new RawProperty { Name = MetaDataProperties.PageName, Value = "Initial" }
                }
            };

            // Act
            subject.Transform(TestUtil.TransferData(content, language1));

            // Assert
            Assert.AreEqual(parent.Name, content.PageName());
            Assert.AreEqual(parent.Name, language1.PageName());
        }

        [TestMethod]
        public void TransformContainer_ShouldUpdateParentLink()
        {
            // Arrange
            const string expected = "EXPECTED";
            var resolver = new Mock<ExportLinkResolver>(null);
            resolver.Setup(x => x.GetExportableLink(It.IsAny<ContentReference>())).Returns(expected);
            var subject = CreateSubject(null, resolver.Object);
            var content = new RawContent { Property = new[] { new RawProperty { Name = MetaDataProperties.PageParentLink, Value = "Initial" } } };

            // Act
            subject.TransformContainer(content, null);

            // Assert
            Assert.AreEqual(expected, content.GetPropertyValue(MetaDataProperties.PageParentLink));
        }

        [TestMethod]
        public void TransformContainer_ShouldSetNameToParentName()
        {
            // Arrange
            var subject = CreateSubject(null, null);
            var content = new RawContent { Property = new[] { new RawProperty { Name = MetaDataProperties.PageName, Value = "Initial" } } };
            var parent = new ComposerPage { Name = "Parent name" };

            // Act
            subject.TransformContainer(content, parent);

            // Assert
            Assert.AreEqual(parent.Name, content.PageName());
        }

        [TestMethod]
        public void TransformContainer_ShouldSetLanguageToParentLanguage()
        {
            // Arrange
            var subject = CreateSubject(null, null);
            var content = new RawContent { Property = new[] { new RawProperty { Name = MetaDataProperties.PageLanguageBranch, Value = "Initial" } } };
            var parent = new ComposerPage { Language = "no" };

            // Act
            subject.TransformContainer(content, parent);

            // Assert
            Assert.AreEqual(parent.Language, content.Language());
        }

        [TestMethod]
        public void TransformContainer_ShouldSetMasterLanguageToParentLanguage()
        {
            // Arrange
            var subject = CreateSubject(null, null);
            var content = new RawContent { Property = new[] { new RawProperty { Name = MetaDataProperties.PageMasterLanguageBranch, Value = "Initial" } } };
            var parent = new ComposerPage { Language = "no" };

            // Act
            subject.TransformContainer(content, parent);

            // Assert
            Assert.AreEqual(parent.Language, content.MasterLanguage());
        }

        private static ComposerContainerTransform CreateSubject(IContentMap contentMap, ExportLinkResolver exportLinkResolver)
        {
            contentMap = contentMap ?? new Mock<IContentMap>().Object;
            exportLinkResolver = exportLinkResolver ?? new Mock<ExportLinkResolver>(null).Object;
            return new ComposerContainerTransform(contentMap, exportLinkResolver, null);
        }
    }
}
