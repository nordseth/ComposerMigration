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
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.DataAbstraction;

namespace EPiServer.ComposerMigration
{
    public class TypeNameTransform : IImportTransform
    {
        private const string ExtensionTypePrefix = "[ExtensionSys]";
        private const string BlockFolderTypeName = "SysContentFolder";

        private readonly MemberNameValidator _memberNameValidator;
        private readonly IComposerImportOptions _options;

        public TypeNameTransform(MemberNameValidator memberNameValidator)
            : this(memberNameValidator, null) { }

        public TypeNameTransform(MemberNameValidator memberNameValidator, IComposerImportOptions options)
        {
            _memberNameValidator = memberNameValidator;
            _options = options ?? ComposerImportOptions.Default;
        }

        public virtual bool Transform(ITransferContentData transferContentData)
        {
            transferContentData.ForAllContent(TransformTypeNames);
            return true;
        }

        private void TransformTypeNames(RawContent rawContent)
        {
            TransformPageTypeName(rawContent);
            // Don't transform page type properties if we are not expecting strongly typed page types
            if (!_options.TransformPageTypeIdentifiers && !rawContent.IsComposerFunction())
            {
                TransformPropertyNames(rawContent);
            }
        }

        /// <summary>
        /// The PageType name will only be changed for specific types as the Import 
        /// matches the DisplayName as well and this will be the same as the old name.
        /// </summary>
        public virtual void TransformPageTypeName(RawContent rawContent)
        {
            var pageTypeProperty = rawContent.GetProperty(MetaDataProperties.PageTypeName);

            if (pageTypeProperty != null)
            {
                string typeName = pageTypeProperty.Value.Trim();
                if (rawContent.IsComposerContainer())
                {
                    typeName = BlockFolderTypeName;
                }
                else if (typeName.StartsWith(ExtensionTypePrefix))
                {
                    typeName = typeName.Substring(ExtensionTypePrefix.Length).TrimStart();
                }

                pageTypeProperty.Value = typeName;
            }
        }

        public virtual void TransformPropertyNames(RawContent raw)
        {
            foreach (var property in raw.Property)
            {
                property.Name = _memberNameValidator.CreateIdentifier(property.Name);
            }
        }
    }
}
