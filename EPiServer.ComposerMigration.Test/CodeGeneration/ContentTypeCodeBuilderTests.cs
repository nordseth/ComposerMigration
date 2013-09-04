using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPiServer.ComposerMigration.DataAbstraction;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.CodeDom;
using Moq;

namespace EPiServer.ComposerMigration.CodeGeneration.Test
{
    [TestClass]
    public class ContentTypeCodeBuilderTests
    {
        [TestMethod]
        public void BuildContentTypeClass_ShouldAlwaysAddTwoNamespaces()
        {
            var subject = CreateSubject();
            var contentType = new ContentType { Name = "MyBlock" };

            var result = subject.BuildContentTypeClass(contentType);

            Assert.AreEqual(2, result.Namespaces.Count);
        }

        [TestMethod]
        public void BuildContentTypeClass_ShouldAlwaysAddOnlyOneType()
        {
            var subject = CreateSubject();
            var contentType = new ContentType { Name = "MyBlock" };

            var result = subject.BuildContentTypeClass(contentType);

            Assert.AreEqual(1, result.Namespaces[1].Types.Count);
        }

        [TestMethod]
        public void BuildContentTypeClass_WhenContentTypeIsBlock_ShouldAddBlockBaseType()
        {
            var contentType = new ContentType { Name = "MyBlock", ContentTypeCategory = ContentTypeCategory.Block };
            var options = new OptionsFake { BlockBaseClass = "BlockBase" };
            var subject = CreateSubject(options);

            var result = subject.BuildContentTypeClass(contentType);

            Assert.IsTrue(result.Namespaces[1].Types[0].BaseTypes.Count > 0);
        }

        [TestMethod]
        public void BuildContentTypeClass_ShouldAddNamespaceFromOptions()
        {
            var contentType = new ContentType { Name = "MyBlock" };
            var options = new OptionsFake { Namespace = "MyCompany" };
            var subject = CreateSubject(options);
            var result = subject.BuildContentTypeClass(contentType);

            Assert.AreEqual("MyCompany", result.Namespaces[ContentTypeCodeBuilder.ClassNamespaceIndex].Name);
        }

        [TestMethod]
        public void BuildContentTypeClass_WhenContentTypeHasNamespace_ShouldCombineNamespaces()
        {
            var contentType = new ContentType { Name = "MyBlock", Namespace = "MyBlocks" };
            var options = new OptionsFake { Namespace = "MyCompany" };
            var subject = CreateSubject(options);
            var result = subject.BuildContentTypeClass(contentType);

            Assert.AreEqual("MyCompany.MyBlocks", result.Namespaces[ContentTypeCodeBuilder.ClassNamespaceIndex].Name);
        }

        [TestMethod]
        public void BuildContentTypeClass_ShouldAddImportsFromOptionsToGlobalNamespace()
        {
            var options = new OptionsFake
            {
                Imports = new[] { "Some.Namespace", "Another.Namespace" }
            };
            var subject = CreateSubject(options);
            var contentType = new ContentType { Name = "MyBlock" };

            var result = subject.BuildContentTypeClass(contentType);

            var resultImports = result.Namespaces[ContentTypeCodeBuilder.GlobalNamespaceIndex].Imports.Cast<CodeNamespaceImport>();

            Assert.AreEqual(options.Imports.Count(), resultImports.Where(x => options.Imports.Contains(x.Namespace)).Count());
        }

        private static ContentTypeCodeBuilder CreateSubject()
        {
            return CreateSubject(null);
        }

        private static ContentTypeCodeBuilder CreateSubject(ICodeGenerationOptions options)
        {
            return new ContentTypeCodeBuilder(new CSharpCodeProvider(), options);
        }
    }
}
