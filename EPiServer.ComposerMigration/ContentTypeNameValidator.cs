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
using System;
using System.Text.RegularExpressions;

namespace EPiServer.ComposerMigration
{
    public class ContentTypeNameValidator
    {
        private const string GroupPattern = @"^\[(.*?)\]\s(.*)$";
        private const string ComposerGroupName = "ExtensionSys";

        private readonly IComposerTranformationOptions _options;
        private readonly MemberNameValidator _memberNameValidator;

        public ContentTypeNameValidator(IComposerTranformationOptions options, MemberNameValidator memberNameValidator)
        {
            _options = options ?? ComposerMigrationOptions.Default;
            _memberNameValidator = memberNameValidator;
        }

        public string CreateContentTypeIdentifier(string typeName, ContentTypeCategory category)
        {
            string groupName;
            return CreateContentTypeIdentifier(typeName, category, out groupName);
        }

        public string CreateContentTypeIdentifier(string typeName, ContentTypeCategory category, out string groupName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                groupName = null;
                return typeName;
            }

            // Check for names that includes a group name, e.g. "[Group name] Page name"
            typeName = RemoveGroupName(typeName, out groupName);

            // Ensure that the type name end with provided suffix
            string suffix = null;
            switch (category)
            {
                case ContentTypeCategory.Page:
                    suffix = _options.PageSuffix;
                    break;
                case ContentTypeCategory.Block:
                    suffix = _options.BlockSuffix;
                    break;
                case ContentTypeCategory.BlockFolder:
                case ContentTypeCategory.Undefined:
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(suffix) && !typeName.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
            {
                typeName += suffix;
            }

            // Create name that is valid code
            typeName = _memberNameValidator.CreateIdentifier(typeName);

            return typeName;
        }

        public string RemoveGroupName(string typeName, out string groupName)
        {
            var match = Regex.Match(typeName, GroupPattern);
            if (!match.Success)
            {
                groupName = null;
                return typeName;
            }

            groupName = match.Groups[1].Value;
            if (groupName == ComposerGroupName)
            {
                groupName = null;
            }

            return match.Groups[2].Value;
        }

    }
}

