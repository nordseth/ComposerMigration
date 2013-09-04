#region Copyright (C) 2013 EPiServer AB
/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this 
software and associated documentation files (the "Software"), to deal in the Software 
without restriction, including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/
#endregion
using EPiServer.ComposerMigration.DataAbstraction;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EPiServer.ComposerMigration.CodeGeneration
{
    public class ContentTypeWriter : IContentTypeWriter
    {
        private const int MaxFileNameLength = 50;
        private static readonly Common.Logging.ILog Logger = Common.Logging.LogManager.GetCurrentClassLogger();
        private readonly IContentTypeCodeBuilder _codeBuilder;
        private readonly CodeDomProvider _codeDomProvider;
        private readonly ICodeGenerationOptions _options;
        private readonly MemberNameValidator _memberNameValidator;

        public ContentTypeWriter(ICodeGenerationOptions options, CodeDomProvider codeDomProvider, IContentTypeCodeBuilder codeBuilder, MemberNameValidator memberNameValidator)
        {
            _options = options ?? (ICodeGenerationOptions)ComposerMigrationOptions.Default;
            _codeDomProvider = codeDomProvider;
            _codeBuilder = codeBuilder;
            _memberNameValidator = memberNameValidator;
        }

        public virtual void Write(ContentType contentType)
        {
            var outputPath = GetOutputFilePath(contentType);

            using (var writer = new CSFilteringStreamWriter(outputPath))
            {
                Logger.InfoFormat("Saving '{0}' to '{1}'", contentType.DisplayName ?? contentType.Name, outputPath);
                Write(contentType, writer);
            }
        }

        public virtual void Write(ContentType contentType, TextWriter writer)
        {
            var generator = _codeDomProvider.CreateGenerator(writer);
            var compileUnit = _codeBuilder.BuildContentTypeClass(contentType);

            generator.GenerateCodeFromCompileUnit(compileUnit, writer, new System.CodeDom.Compiler.CodeGeneratorOptions { BracingStyle = "C" });
        }

        public virtual string GetOutputFilePath(ContentType contentType)
        {
            var fileName = contentType.Name;

            if (fileName.Length > MaxFileNameLength)
            {
                fileName = fileName.Substring(0, MaxFileNameLength);
            }

            fileName += "." + _codeDomProvider.FileExtension.TrimStart('.');

            // Create a subfolder structure base on ContentType Namespace.
            var namespacePath = (contentType.Namespace ?? "").Replace('.', Path.DirectorySeparatorChar);

            var directoryPath = Path.Combine((_options.OutputDirectory ?? ""), namespacePath);

            // Ensure that the path exists
            Directory.CreateDirectory(directoryPath);

            return Path.Combine(directoryPath, fileName);
        }

        /// <summary>
        /// This class is used to compensate for Auto-Generated property hack.
        /// </summary>
        private class CSFilteringStreamWriter : StreamWriter
        {
            private const string VirtualMarker = "const "; // We never use 
            private const string AutoPropertyBody = "{ get; set; }";
            private bool _autoProperty;

            public CSFilteringStreamWriter(string path)
                : base(path) { }

            public override void Write(string value)
            {
                if (value == VirtualMarker)
                {
                    value = "virtual ";
                }
                else
                {
                    value = ReplaceNullable(value);
                    _autoProperty = value.EndsWith(AutoPropertyBody);
                }
                base.Write(value);
            }

            private static string ReplaceNullable(string value)
            {
                return Regex.Replace(value, @"System.Nullable<(System\.)?([^>]+)>", "$2?");
            }

            public override void Write(char[] buffer, int index, int count)
            {
                if (_autoProperty)
                {
                    // Strip the rouge semi-colon stemming from our fake auto-property implementation
                    if (buffer.Length > 0 && buffer[0] == ';')
                    {
                        buffer = buffer.Skip(1).ToArray();
                        count--;
                    }
                    _autoProperty = false;
                }
                base.Write(buffer, index, count);
            }
        }

    }
}
