using EPiServer.ComposerMigration.DataAbstraction;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.Core.Transfer;
using EPiServer.DataAbstraction;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class ContentAreaTransformTests
    {
        private static readonly ILookup<string, ComposerContentFunction> EmptyContentFunctionLookup = Enumerable.Empty<ComposerContentFunction>().ToLookup(x => (string)null);

        [TestMethod]
        public void Transform_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var subject = CreateSubject();

            // Act
            var result = subject.Transform(TestUtil.TransferData(new RawContent()));

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TransformContentAreas_ShouldChangeTypeOnAllContentAreas()
        {
            // Arrange
            const string AreaName = "Area1";
            var subject = CreateSubject();
            var content = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = AreaName, TypeName = ComposerPropertyTypes.ContentArea },
                    new RawProperty { Name = MetaDataProperties.PageParentLink, Value = "Initial" } 
                }
            };

            // Act
            subject.TransformContentAreas(content);

            // Assert
            Assert.AreEqual(typeof(PropertyContentArea).FullName, content.GetProperty(AreaName).TypeName);
        }

        [TestMethod]
        public void PopulateContentArea_ShouldAddContentFragmentsToPropertyValue()
        {
            // Arrange
            var subject = CreateSubject(fragmentBuilder: CreateFragmentBuilder());
            var property = new RawProperty();
            var guid = Guid.NewGuid();
            var functions = new[] { new ComposerContentFunction { Guid = guid } };

            // Act
            subject.PopulateContentArea(property, functions);

            // Assert
            Assert.IsFalse(property.IsNull);
            Assert.AreEqual(guid.ToString(), property.Value);
        }

        [TestMethod]
        public void PopulateContentArea_WithMultipleFunctions_ShouldAddContentFragmentsForEachFunctionToPropertyValue()
        {
            // Arrange
            var subject = CreateSubject(fragmentBuilder: CreateFragmentBuilder());
            var property = new RawProperty();
            var guids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var functions = new[] 
            { 
                new ComposerContentFunction { Guid = guids[0] },
                new ComposerContentFunction { Guid = guids[1] },
                new ComposerContentFunction { Guid = guids[2] } 
            };

            // Act
            subject.PopulateContentArea(property, functions);

            // Assert
            for (int i = 0; i < guids.Length; i++)
            {
                Assert.IsTrue(property.Value.Contains(guids[i].ToString()));
            }
        }

        private static ContentFragmentBuilder CreateFragmentBuilder()
        {
            var fragmentBuilder = new Mock<ContentFragmentBuilder>(null, null);
            fragmentBuilder.Setup(x => x.CreateFragment(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>()))
                .Returns<Guid, string, IEnumerable<Guid>>((a, b, c) => CreateFragment(a.ToString()));

            return fragmentBuilder.Object;
        }

        private static ContentFragment CreateFragment(string internalFormat)
        {
            var mocker = new AutoMocker();
            mocker.Use<TemplateModelSelector>(mocker.CreateInstance<TemplateModelSelector>());
            mocker.Use<DisplayChannelService>(mocker.CreateInstance<DisplayChannelService>());
            mocker.Use<TemplateModelRepository>(mocker.CreateInstance<TemplateModelRepository>());
            mocker.Use<TemplateResolver>(mocker.CreateInstance<TemplateResolver>());
            mocker.Use<TemplateControlLoader>(mocker.CreateInstance<TemplateControlLoader>());

            var fragment = new Mock<ContentFragment>(Mock.Of<IContentLoader>(), mocker.CreateInstance<TemplateControlLoader>(), Mock.Of<ISecuredFragmentMarkupGenerator>());
            fragment.SetupGet(x => x.InternalFormat).Returns(internalFormat);
            return fragment.Object;
        }

        private static ContentAreaTransform CreateSubject(IContentMap contentMap = null, IContentTransferContext transferContext = null, ContentFragmentBuilder fragmentBuilder = null)
        {
            if (contentMap == null)
            {
                var mock = new Mock<IContentMap>();
                mock.Setup(x => x.GetContentFunctions(It.IsAny<Guid>(), It.IsAny<string>())).Returns(EmptyContentFunctionLookup);
                contentMap = mock.Object;
            }
            transferContext = transferContext ?? new Mock<IContentTransferContext>().Object;
            fragmentBuilder = fragmentBuilder ?? new Mock<ContentFragmentBuilder>(null, null).Object;
            return new ContentAreaTransform(contentMap, fragmentBuilder, transferContext);
        }
    }
}
