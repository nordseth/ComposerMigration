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
    public class ComposerFunctionTransformTests
    {
        [TestMethod]
        public void Transform_WhenFunctionIsGlobal_ShouldUpdateParentLink()
        {
            // Arrange
            const string expected = "EXPECTED";
            var resolver = new Mock<ExportLinkResolver>(null);
            resolver.Setup(x => x.GetExportableLink(It.IsAny<ContentReference>())).Returns(expected);
            var functionInfo = new ComposerContentFunctionInformation { Type = ComposerContentFunctionCategory.Global };
            var subject = CreateSubject(functionInfo: functionInfo, exportLinkResolver: resolver.Object);
            var content = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.ContentFunction },
                    new RawProperty { Name = MetaDataProperties.PageParentLink, Value = "Initial" } 
                }
            };

            // Act
            subject.Transform(TestUtil.TransferData(content));

            // Assert
            Assert.AreEqual(expected, content.GetPropertyValue(MetaDataProperties.PageParentLink));
        }

        [TestMethod]
        public void Transform_WhenFunctionIsLocal_ShouldNotTouchParentLink()
        {
            // Arrange
            const string expected = "Initial";
            var functionInfo = new ComposerContentFunctionInformation { Type = ComposerContentFunctionCategory.Content };
            var subject = CreateSubject(functionInfo: functionInfo);
            var content = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.ContentFunction },
                    new RawProperty { Name = MetaDataProperties.PageParentLink, Value = expected } 
                }
            };

            // Act
            subject.Transform(TestUtil.TransferData(content));

            // Assert
            Assert.AreEqual(expected, content.GetPropertyValue(MetaDataProperties.PageParentLink));
        }

        [TestMethod]
        public void Transform_WhenFunctionIsLocal_ShouldSetLanguageToParentLanguage()
        {
            // Arrange
            var parent = new ComposerPage { Language = "no" };
            var contentMap = new Mock<IContentMap>();
            contentMap.Setup(x => x.GetParentPage(It.IsAny<Guid>())).Returns(parent);

            var functionInfo = new ComposerContentFunctionInformation { Type = ComposerContentFunctionCategory.Content };
            var subject = CreateSubject(contentMap.Object, functionInfo);
            var content = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.ContentFunction },
                    new RawProperty { Name = MetaDataProperties.PageLanguageBranch, Value = "sv" } 
                }
            };

            // Act
            subject.Transform(TestUtil.TransferData(content));

            // Assert
            Assert.AreEqual(parent.Language, content.Language());
        }

        [TestMethod]
        public void Transform_WhenFunctionIsLocal_ShouldSetMasterLanguageToParentLanguage()
        {
            // Arrange
            var parent = new ComposerPage { Language = "no" };
            var contentMap = new Mock<IContentMap>();
            contentMap.Setup(x => x.GetParentPage(It.IsAny<Guid>())).Returns(parent);

            var functionInfo = new ComposerContentFunctionInformation { Type = ComposerContentFunctionCategory.Content };
            var subject = CreateSubject(contentMap.Object, functionInfo);
            var content = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.ContentFunction },
                    new RawProperty { Name = MetaDataProperties.PageMasterLanguageBranch, Value = "sv" } 
                }
            };

            // Act
            subject.Transform(TestUtil.TransferData(content));

            // Assert
            Assert.AreEqual(parent.Language, content.MasterLanguage());
        }

        [TestMethod]
        public void CreateFriendlyName_WhenFunctionInfoHasName_ShouldReturnInfoName()
        {
            // Arrange
            var functionInfo = new ComposerContentFunctionInformation { Name = "Info name" };
            var subject = CreateSubject(functionInfo: functionInfo);

            // Act
            var result = subject.CreateFriendlyName(null, functionInfo);

            // Assert
            Assert.AreEqual(functionInfo.Name, result);
        }

        [TestMethod]
        public void CreateFriendlyName_ShouldBeginWithTypeName()
        {
            // Arrange
            const string typeName = "MyType";
            var subject = CreateSubject();
            var content = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.ContentFunction },
                    new RawProperty { Name = MetaDataProperties.PageTypeName, Value = typeName } 
                }
            };

            // Act
            var result = subject.CreateFriendlyName(content, null);

            // Assert
            Assert.IsTrue(result.StartsWith(typeName));
        }

        [TestMethod]
        public void CreateFriendlyName_WhenHeadingPropertyExists_ShouldGetNameFromTypeAndHeadingValue()
        {
            // Arrange
            const string typeName = "MyType";
            const string heading = "My Heading";
            const string headingProperty = "MyHeadingName";
            var options = new Mock<IComposerImportOptions>();
            options.SetupGet(x => x.BlockNameProperties).Returns(new [] { headingProperty });
            var subject = CreateSubject(options: options.Object);
            var content = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.ContentFunction },
                    new RawProperty { Name = MetaDataProperties.PageTypeName, Value = typeName },
                    new RawProperty { Name = headingProperty, Value = heading } 
                }
            };

            // Act
            var result = subject.CreateFriendlyName(content, null);

            // Assert
            Assert.AreEqual(string.Format("{0} - {1}", typeName, heading), result);
        }

        private static ComposerSerializer CreateSerializer(ComposerContentFunctionInformation info)
        {
            var serializer = new Mock<ComposerSerializer>();
            serializer.Setup(x => x.Deserialize<ComposerContentFunctionInformation>(It.IsAny<string>())).Returns(info);
            return serializer.Object;
        }

        private static ComposerFunctionTransform CreateSubject(IContentMap contentMap = null, ComposerContentFunctionInformation functionInfo = null, ExportLinkResolver exportLinkResolver = null, IComposerImportOptions options = null)
        {
            contentMap = contentMap ?? new Mock<IContentMap>().Object;
            exportLinkResolver = exportLinkResolver ?? new Mock<ExportLinkResolver>(null).Object;
            return new ComposerFunctionTransform(contentMap, CreateSerializer(functionInfo), exportLinkResolver, options);
        }
    }
}
