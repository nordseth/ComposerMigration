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
    public class ContentMapCollectorTests
    {
        [TestMethod]
        public void Collect_WhenContentIsComposerPage_ShouldAddPageToContentMap()
        {
            // Arrange
            var contentMap = new Mock<IContentMap>();
            var pageInfo = new ComposerPageInformation { Page = new ComposerPage { Guid = Guid.NewGuid() } };
            var subject = CreateSubject(contentMap.Object, CreateSerializer(pageInfo));

            var content = new RawContent { Property = new[] { new RawProperty { Name = ComposerProperties.Page } } };

            // Act
            subject.Collect(TestUtil.TransferData(content));

            // Assert
            contentMap.Verify(x => x.AddPage(pageInfo.Page), Times.Once());
        }

        [TestMethod]
        public void Collect_WhenMultipleLanguagesExist_ShouldAddPageToContentMapForEachLanguage()
        {
            // Arrange
            var contentMap = new Mock<IContentMap>();
            var pageInfo = new ComposerPageInformation { Page = new ComposerPage { Guid = Guid.NewGuid() } };
            var subject = CreateSubject(contentMap.Object, CreateSerializer(pageInfo));

            var master = new RawContent { Property = new[] { new RawProperty { Name = ComposerProperties.Page } } };
            var language1 = new RawContent { Property = new[] { new RawProperty { Name = ComposerProperties.Page } } };
            var language2 = new RawContent { Property = new[] { new RawProperty { Name = ComposerProperties.Page } } };

            // Act
            subject.Collect(TestUtil.TransferData(master, language1, language2));

            // Assert
            contentMap.Verify(x => x.AddPage(pageInfo.Page), Times.Exactly(3));
        }

        [TestMethod]
        public void TryMap_ShouldSetNameFromPageName()
        {
            // Arrange
            var contentMap = new Mock<IContentMap>();
            var pageInfo = new ComposerPageInformation { Page = new ComposerPage { Guid = Guid.NewGuid() } };
            var subject = CreateSubject(contentMap.Object, CreateSerializer(pageInfo));

            var pageName = "TestName";
            var content = new RawContent
                {
                    Property = new[] 
                        { 
                            new RawProperty { Name = ComposerProperties.Page }, 
                            new RawProperty { Name = MetaDataProperties.PageName, Value = pageName } 
                        }
                };

            // Act
            IComposerPage result;
            if (!subject.TryMap(content, out result))
            {
                Assert.Fail();
            }

            // Assert
            Assert.AreEqual(pageName, result.Name);
        }

        [TestMethod]
        public void TryMap_ShouldSetLanguageFromPageLanguage()
        {
            // Arrange
            var contentMap = new Mock<IContentMap>();
            var pageInfo = new ComposerPageInformation { Page = new ComposerPage { Guid = Guid.NewGuid() } };
            var subject = CreateSubject(contentMap.Object, CreateSerializer(pageInfo));

            var lang = "no";
            var content = new RawContent
            {
                Property = new[] 
                        { 
                            new RawProperty { Name = ComposerProperties.Page }, 
                            new RawProperty { Name = MetaDataProperties.PageLanguageBranch, Value = lang } 
                        }
            };

            // Act
            IComposerPage result;
            if (!subject.TryMap(content, out result))
            {
                Assert.Fail();
            }

            // Assert
            Assert.AreEqual(lang, result.Language);
        }

        private static ComposerSerializer CreateSerializer(ComposerPageInformation info)
        {
            var serializer = new Mock<ComposerSerializer>();
            serializer.Setup(x => x.Deserialize<ComposerPageInformation>(It.IsAny<string>())).Returns(info);
            return serializer.Object;
        }

        private static ContentMapCollector CreateSubject(IContentMap contentMap, ComposerSerializer serializer)
        {
            contentMap = contentMap ?? new Mock<IContentMap>().Object;
            if (serializer == null)
            {
                var pageInfo = new ComposerPageInformation { Page = new ComposerPage { Guid = Guid.NewGuid() } };
                serializer = CreateSerializer(pageInfo);
            }
            return new ContentMapCollector(contentMap, serializer);
        }
    }
}
