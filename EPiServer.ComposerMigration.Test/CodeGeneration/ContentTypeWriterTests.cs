using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPiServer.ComposerMigration.DataAbstraction;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using Moq;

namespace EPiServer.ComposerMigration.CodeGeneration.Test
{
    [TestClass]
    public class ContentTypeWriterTests
    {
        private static ContentTypeWriter CreateSubject()
        {
            return new ContentTypeWriter(null, 
                new CSharpCodeProvider(),
                new ContentTypeCodeBuilder(new CSharpCodeProvider(), null), 
                new MemberNameValidator());
        }

        [TestMethod]
        public void Write_ShouldIncludeClass()
        {
            var subject = CreateSubject();
            var contentType = new ContentType { Name = "SomeName" };

            var result = GetString(sw => subject.Write(contentType, sw));

            Assert.IsTrue(result.Contains("public class SomeName"));
        }

        [TestMethod]
        public void GetOutputFilePath_ShouldLimitFilenameLengthTo50Chars()
        {
            var subject = CreateSubject();
            var contentType = new ContentType { Name = "Some very long name that would really ridicuolus to use as a page type name but still we should still test for it" };

            var result = subject.GetOutputFilePath(contentType);

            var fileName = Path.GetFileNameWithoutExtension(result);

            Assert.IsFalse(fileName.Length > 50);
        }

        private static string GetString(Action<StringWriter> action)
        {
            var output = new StringBuilder();
            using (var stringWriter = new StringWriter(output))
            {
                action(stringWriter);
            }
            return output.ToString();
        }
    }
}
