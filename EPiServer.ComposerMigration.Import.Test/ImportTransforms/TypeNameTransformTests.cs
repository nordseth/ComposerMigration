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
    public class TypeNameTransformTests
    {
        [TestMethod]
        public void TransformPageTypeName_WhenContentIsContainer_ShouldChangeTypeToFolder()
        {
            // Arrange
            var subject = CreateSubject(null, null);

            var content = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.ContainerPage }, 
                    new RawProperty { Name = MetaDataProperties.PageTypeName, Value = "Initial value" } 
                }
            };

            // Act
            subject.TransformPageTypeName(content);

            // Assert
            Assert.AreEqual("SysContentFolder", content.GetPropertyValue(MetaDataProperties.PageTypeName));
        }

        [TestMethod]
        public void TransformPageTypeName_WhenNameContainsExtensionSysPrefix_ShouldRemovePrefix()
        {
            // Arrange
            var subject = CreateSubject(null, null);

            var content = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.ContentFunction }, 
                    new RawProperty { Name = MetaDataProperties.PageTypeName, Value = "[ExtensionSys] My page type" } 
                }
            };

            // Act
            subject.TransformPageTypeName(content);

            // Assert
            Assert.AreEqual("My page type", content.GetPropertyValue(MetaDataProperties.PageTypeName));
        }

        private static TypeNameTransform CreateSubject(MemberNameValidator nameValidator, IComposerImportOptions options)
        {
            nameValidator = nameValidator ?? new Mock<MemberNameValidator>().Object;
            return new TypeNameTransform(nameValidator, options);
        }
    }
}
