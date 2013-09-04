using EPiServer.ComposerMigration.DataAbstraction;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class ContentTypeNameValidatorTests
    {
        [TestMethod]
        public void CreateContentTypeIdentifier_WhenTypeNameUsesGroupNameConvention_ShouldRemoveFromName()
        {
            var typeName = "[My Group] SomePage";
            var subject = CreateSubject();

            var result = subject.CreateContentTypeIdentifier(typeName, ContentTypeCategory.Undefined);

            Assert.AreEqual("SomePage", result);
        }

        [TestMethod]
        public void CreateContentTypeIdentifier_WhenTypeNameUsesGroupNameConvention_ShouldMapToGroupName()
        {
            var typeName = "[My Group] SomePage";
            var subject = CreateSubject();

            string result;
            subject.CreateContentTypeIdentifier(typeName, ContentTypeCategory.Undefined, out result);

            Assert.AreEqual("My Group", result);
        }

        [TestMethod]
        public void CreateContentTypeIdentifier_WhenTypeNameHasComposerGroupName_ShouldNotSetGroupName()
        {
            var typeName = "[ExtensionSys] SomePage";
            var subject = CreateSubject();

            string result;
            subject.CreateContentTypeIdentifier(typeName, ContentTypeCategory.Undefined, out result);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void CreateContentTypeIdentifier_WhenCategoryIsBlock_ShouldAlwaysEndWithProvidedBlockSuffix()
        {
            var options = new OptionsFake { BlockSuffix = "Blockskij" };
            var typeName = "Name";
            var subject = CreateSubject(options);

            var result = subject.CreateContentTypeIdentifier(typeName, ContentTypeCategory.Block);

            Assert.IsTrue(result.EndsWith(options.BlockSuffix));
        }

        [TestMethod]
        public void EnsureValidNaming_WhenCategoryIsPage_ShouldAlwaysEndWithProvidedPageSuffix()
        {
            var options = new OptionsFake { PageSuffix = "Pageskij" };
            var typeName = "Name";
            var subject = CreateSubject(options);

            var result = subject.CreateContentTypeIdentifier(typeName, ContentTypeCategory.Page);

            Assert.IsTrue(result.EndsWith(options.PageSuffix));
        }

        [TestMethod]
        public void CreateContentTypeIdentifier_WhenNameEndsWithSuffix_ShouldNotModifyName()
        {
            var options = new OptionsFake { PageSuffix = "Pageskij" };
            var typeName = "NamePageskij";
            var subject = CreateSubject(options);

            var result = subject.CreateContentTypeIdentifier(typeName, ContentTypeCategory.Page);

            Assert.AreEqual(typeName, result);
        }

        [TestMethod]
        public void CreateContentTypeIdentifier_WhenNameEndsWithDifferentlyCasedSuffix_ShouldNotDoubleUpSuffix()
        {
            var options = new OptionsFake { PageSuffix = "Pageskij" };
            var typeName = "Name pageskij";
            var subject = CreateSubject(options);

            var result = subject.CreateContentTypeIdentifier(typeName, ContentTypeCategory.Page);

            Assert.AreEqual("NamePageskij", result);
        }

        private static ContentTypeNameValidator CreateSubject(IComposerTranformationOptions options)
        {
            return new ContentTypeNameValidator(options, new MemberNameValidator());
        }

        private static ContentTypeNameValidator CreateSubject()
        {
            return CreateSubject(null);
        }
    }
}
