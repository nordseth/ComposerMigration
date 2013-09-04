using EPiServer.ComposerMigration.DataAbstraction;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class ContentTypeMapperTests
    {
        [TestMethod]
        public void Map_ShouldMapAllSimpleProperties()
        {
            var source = new PageType
            {
                Name = "SomePage",
                Description = "Some description",
                GUID = Guid.NewGuid(),
                IsAvailable = true,
                SortOrder = 43
            };

            var subject = CreateSubject();

            var result = subject.Map(source);

            Assert.AreEqual(source.Name, result.Name);
            Assert.AreEqual(source.Description, result.Description);
            Assert.AreEqual(source.GUID, result.GUID);
            Assert.AreEqual(source.IsAvailable, result.IsAvailable);
            Assert.AreEqual(source.SortOrder, result.SortOrder);
        }

        [TestMethod]
        public void Map_ShouldMapACL()
        {
            var source = new PageType
            {
                Name = "Name",
                ACL = new AccessControlList
                {
                    Entries = new List<AccessControlEntry> { 
                        new AccessControlEntry { Name = "Everyone", EntityType = SecurityEntityType.User, Access = AccessLevel.Create } 
                    }
                }
            };
            var subject = CreateSubject();

            var result = subject.Map(source);

            Assert.AreEqual(1, result.ACL.Entries.Count);
            Assert.AreEqual("Everyone", result.ACL.Entries[0].Name);
        }

        [TestMethod]
        public void Map_WhenFileNameIsUserControl_ShouldSetIsBlockType()
        {
            var source = new PageType
            {
                Name = "MyBlock",
                ExportableFileName = "~/Templates/Blocks/MyBlock.ascx"
            };
            var subject = CreateSubject();

            var result = subject.Map(source);

            Assert.AreEqual(ContentTypeCategory.Block, result.ContentTypeCategory);
        }

        [TestMethod]
        public void Map_WhenNameIsNotValidIdentifier_ShouldSetDisplayNameToPageTypeName()
        {
            var source = new PageType { Name = "My Page" };

            var subject = CreateSubject();

            var result = subject.Map(source);

            Assert.AreEqual(source.Name, result.DisplayName);
        }

        [TestMethod]
        public void Map_WhenNameHasGroup_ShouldSetDisplayNameToPageTypeNameWithoutGroup()
        {
            var source = new PageType { Name = "[My group] Page" };

            var subject = CreateSubject();

            var result = subject.Map(source);

            Assert.AreEqual(source.Name, result.DisplayName);
        }

        [TestMethod]
        public void Map_WhenNameHasExtensionGroup_ShouldRemoveGroupFromDisplayName()
        {
            var source = new PageType { Name = "[ExtensionSys] Page" };

            var subject = CreateSubject();

            var result = subject.Map(source);

            Assert.AreEqual("Page", result.DisplayName);
        }

        [TestMethod]
        public void Map_NameIsNotChanged_ShouldNotSetDisplayName()
        {
            var source = new PageType { Name = "Homepage" };

            var subject = CreateSubject();

            var result = subject.Map(source);

            Assert.IsNull(result.DisplayName);
        }

        [TestMethod]
        public void Map_ShouldFilterOutComposerSystemProperties()
        {
            var validProperty = "MainBody";
            var definitionType = new PageDefinitionType { DataType = "String" };
            var source = new PageType
            {
                Name = "SomePage",
                GUID = Guid.NewGuid(),
                Definitions =
                {
                    new PageDefinition { Name = validProperty, Type = definitionType },
                    new PageDefinition { Name = ComposerProperties.Page, Type = definitionType },
                    new PageDefinition { Name = ComposerProperties.ContainerPage, Type = definitionType },
                    new PageDefinition { Name = ComposerProperties.ContentFunction, Type = definitionType },
                    new PageDefinition { Name = ComposerProperties.NeverUsed, Type = definitionType },
                    new PageDefinition { Name = ComposerProperties.PersonalizationData, Type = definitionType }
                }
            };

            var subject = CreateSubject();

            var result = subject.Map(source);

            Assert.AreEqual(validProperty, result.PropertyDefinitions.Single().Name);
        }

        [TestMethod]
        public void Map_WhenTypeIsBlock_ShouldSetIsAvailableToTrue()
        {
            var source = new PageType
            {
                Name = "MyBlock",
                IsAvailable = false,
                ExportableFileName = "~/Templates/Blocks/MyBlock.ascx"
            };

            var subject = CreateSubject();

            var result = subject.Map(source);

            Assert.IsTrue(result.IsAvailable);
        }

        private static ContentTypeMapper CreateSubject(IComposerTranformationOptions options = null)
        {
            return new ContentTypeMapper(new PropertyMapper(new MemberNameValidator()), options, new ContentTypeNameValidator(options, new MemberNameValidator()), new MemberNameValidator());
        }
    }
}
