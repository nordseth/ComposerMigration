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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EPiServer.ComposerMigration
{
    public class ContentTypeMapper : IContentTypeMapper
    {
        private const string UserControlExtension = ".ascx";
        
        private static readonly ICollection<string> IgnoredPropertyNames = new HashSet<string>(new[] { 
            ComposerProperties.Page, 
            ComposerProperties.ContainerPage, 
            ComposerProperties.ContentFunction, 
            ComposerProperties.NeverUsed,
            ComposerProperties.PersonalizationData
        }, StringComparer.InvariantCultureIgnoreCase);


        private readonly IComposerTranformationOptions _options;
        private readonly IPropertyMapper _propertyMapper;
        private readonly ContentTypeNameValidator _contentTypeNameValidator;
        private readonly MemberNameValidator _memberNameValidator;

        public ContentTypeMapper(IPropertyMapper propertyMapper, IComposerTranformationOptions options, ContentTypeNameValidator contentTypeNameValidator, MemberNameValidator memberNameValidator)
        {
            _propertyMapper = propertyMapper;
            _options = options ?? ComposerMigrationOptions.Default;
            _contentTypeNameValidator = contentTypeNameValidator;
            _memberNameValidator = memberNameValidator;
        }

        public virtual ContentType Map(PageType pageType)
        {
            if (pageType == null)
                throw new ArgumentNullException("pageType");

            if (string.IsNullOrEmpty(pageType.Name))
                throw new ArgumentException("PageType must have a name.", "pageType");

            var contentTypeCategory = GetContentTypeCategory(pageType);

            var contentType = new ContentType
            {
                Name = pageType.Name,
                Description = pageType.Description,
                GUID = pageType.GUID,
                // As all Composer Function page types are set as not available we reverse this here
                IsAvailable = pageType.IsAvailable || contentTypeCategory == ContentTypeCategory.Block,
                SortOrder = pageType.SortOrder,
                ACL = pageType.ACL,
                ContentTypeCategory = contentTypeCategory,
                Defaults = pageType.Defaults
            };

            EnsureValidNaming(contentType);

            // Set DisplayName to original name
            if (contentType.Name != pageType.Name)
            {
                contentType.DisplayName = pageType.Name.StartsWith(ComposerPageTypes.Prefix) ? pageType.Name.Substring(ComposerPageTypes.Prefix.Length).TrimStart() : pageType.Name;
            }

            contentType.Namespace = CalculateNamespace(contentType);
            MapProperties(pageType, contentType);
            return contentType;
        }

        private void EnsureValidNaming(ContentType contentType)
        {
            string groupName;
            contentType.Name = _contentTypeNameValidator.CreateContentTypeIdentifier(contentType.Name, contentType.ContentTypeCategory, out groupName);
            contentType.GroupName = groupName;
        }

        private ContentTypeCategory GetContentTypeCategory(PageType pageType)
        {
            // If the filename ends with .ascx we assume its a function type
            if (pageType.ExportableFileName != null && pageType.ExportableFileName.EndsWith(UserControlExtension, StringComparison.OrdinalIgnoreCase))
            {
                return ContentTypeCategory.Block;
            }
            return ContentTypeCategory.Page;
        }

        private string CalculateNamespace(ContentType contentType)
        {
            var ns = new List<string>();

            // Group namespace
            if (_options.IncludeGroupNameInNamespace && !string.IsNullOrEmpty(contentType.GroupName))
            {
                var groupNs = _memberNameValidator.CreateIdentifier(contentType.GroupName);
                ns.Add(groupNs);
            }

            // Type namespace
            switch (contentType.ContentTypeCategory)
            {
                case ContentTypeCategory.Page:
                    ns.Add(_options.PageNamespace);
                    break;
                case ContentTypeCategory.Block:
                    ns.Add(_options.BlockNamespace);
                    break;
                default:
                    break;
            }

            return string.Join(".", ns);
        }

        private void MapProperties(PageType pageType, ContentType contentType)
        {
            foreach (var pageDefinition in pageType.Definitions.Where(SupportedPropertiesPredicate))
            {
                var propertyDefinition = _propertyMapper.Map(pageDefinition);
                if (propertyDefinition != null)
                {
                    contentType.PropertyDefinitions.Add(propertyDefinition);
                }
            }
        }

        private bool SupportedPropertiesPredicate(PageDefinition definition)
        {
            return !IgnoredPropertyNames.Contains(definition.Name);
        }

    }
       
}

