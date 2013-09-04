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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.ComposerMigration.DataAbstraction;

namespace EPiServer.ComposerMigration
{
    public class PropertyMapper : IPropertyMapper
    {
        private const string LegacyWrapperUIHint = "LegacyWrapper"; // This is just a UIHint that should not have any mappings - which will trigger the Legacy system.

        private static readonly Dictionary<string, PropertyDefinitionType> DefaultTypeMapping = new Dictionary<string, PropertyDefinitionType>
        {
            { "Boolean", new PropertyDefinitionType { Type = typeof(bool) }},
            { "Category", new PropertyDefinitionType { TypeName = "EPiServer.Core.CategoryList" }},
            { "Date" , new PropertyDefinitionType { Type = typeof(DateTime) }},
            { "FloatNumber",new PropertyDefinitionType { Type = typeof(double) }},
            { "LinkCollection" , new PropertyDefinitionType { TypeName = "EPiServer.SpecializedProperties.LinkItemCollection" } },
            { "Number" , new PropertyDefinitionType { Type = typeof(int) }},
            { "PageReference" ,new PropertyDefinitionType { TypeName = "EPiServer.Core.PageReference" }},
            { "PageType" , new PropertyDefinitionType { TypeName = "EPiServer.DataAbstraction.PageType" } },
            { "String" , new PropertyDefinitionType { Type = typeof(string) }},
            { "LongString" , new PropertyDefinitionType { Type = typeof(string), UIHint = "$UIHint.Textarea" }},
            { "EPiServer.SpecializedProperties.PropertyContentArea" , new PropertyDefinitionType { TypeName = "EPiServer.Core.ContentArea" }},
            { "EPiServer.SpecializedProperties.PropertyTimeSpan" , new PropertyDefinitionType { Type = typeof(TimeSpan) }},
            { "EPiServer.SpecializedProperties.PropertyUrl" , new PropertyDefinitionType { TypeName = "EPiServer.Url" }},
            { "EPiServer.SpecializedProperties.PropertyImageUrl" , new PropertyDefinitionType { TypeName = "EPiServer.Url", UIHint = "$UIHint.Image" }},
            { "EPiServer.SpecializedProperties.PropertyDocumentUrl" , new PropertyDefinitionType { TypeName = "EPiServer.Url", UIHint = "$UIHint.Document" }},
            { "EPiServer.Core.PropertyXForm" , new PropertyDefinitionType { TypeName = "EPiServer.XForms.XForm" }},
            { "EPiServer.SpecializedProperties.PropertyXhtmlString" , new PropertyDefinitionType { TypeName = "EPiServer.Core.XhtmlString" }},
            { ComposerPropertyTypes.ContentArea , new PropertyDefinitionType { TypeName = "EPiServer.Core.ContentArea" }},
            { ComposerPropertyTypes.FunctionPicker, new PropertyDefinitionType { TypeName = "EPiServer.Core.ContentReference" }},
        };

        private static readonly Dictionary<Type, Type> NullableMapping = new Dictionary<Type, Type>
        {
            { typeof(int), typeof(int?) },
            { typeof(double), typeof(double?) },
            { typeof(DateTime), typeof(DateTime?) },
            { typeof(TimeSpan), typeof(TimeSpan?) }
        };

        private static readonly Dictionary<string, string> GroupMappings = new Dictionary<string, string>
        {
            { "Categories", "$SystemTabNames.Categories" },
            { "Information", "$SystemTabNames.Content" },
            { "Scheduling", "$SystemTabNames.Scheduling" },
            { "Advanced", "$SystemTabNames.Settings" },
            { "Shortcut", "$SystemTabNames.Shortcut" },
            { "Extension", "$SystemTabNames.Content" }
        };

        private readonly MemberNameValidator _memberNameValidator;
        private readonly IComposerTranformationOptions _options;

        public PropertyMapper(MemberNameValidator memberNameValidator)
            : this(memberNameValidator, null) { }

        public PropertyMapper(MemberNameValidator memberNameValidator, IComposerTranformationOptions options)
        {
            _memberNameValidator = memberNameValidator;
            _options = options ?? ComposerMigrationOptions.Default;
        }

        public virtual PropertyDefinition Map(PageDefinition pageDefinition)
        {
            var propertyDefinition = new PropertyDefinition
            {
                Name = _memberNameValidator.CreateIdentifier(pageDefinition.Name),
                DisplayEditUI = pageDefinition.DisplayEditUI,
                EditCaption = pageDefinition.EditCaption,
                FieldOrder = pageDefinition.FieldOrder,
                HelpText = pageDefinition.HelpText,
                LanguageSpecific = pageDefinition.LanguageSpecific,
                Required = pageDefinition.Required,
                Searchable = pageDefinition.Searchable,
                GroupName = MapGroupName(pageDefinition.Tab.Name),
                Type = MapType(pageDefinition.Type)
            };

            ModifyComposerPropertyTypes(pageDefinition, propertyDefinition);

            return propertyDefinition;
        }

        public virtual PropertyDefinitionType MapType(PageDefinitionType pageDefinitionType)
        {
            // Replace types that has a Default Backing Types mapping with mapped type
            PropertyDefinitionType mapped;
            if (!string.IsNullOrEmpty(pageDefinitionType.TypeName) && DefaultTypeMapping.TryGetValue(pageDefinitionType.TypeName, out mapped))
            {
                mapped = mapped.Copy();
            }
            else
            {
                // All DataTypes have a mapping
                mapped = DefaultTypeMapping[pageDefinitionType.DataType].Copy();

                if (!string.IsNullOrEmpty(pageDefinitionType.TypeName))
                {
                    mapped.BackingTypeName = pageDefinitionType.TypeName;
                    // Any previously set UIHint will applies to the base type, we want to ensure that we are using legacy mode here!
                    mapped.UIHint = _options.ForceLegacyPropertyWrapper ? LegacyWrapperUIHint : null;
                }
            }

            Type nullable;
            if (_options.UseNullableValueTypes && mapped.Type != null && NullableMapping.TryGetValue(mapped.Type, out nullable))
            {
                mapped.Type = nullable;
            }

            return mapped;
        }

        public virtual string MapGroupName(string tabName)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                return tabName;
            }

            string mapped;
            if (GroupMappings.TryGetValue(tabName, out mapped))
            {
                return mapped;
            }
            return tabName;
        }

        private void ModifyComposerPropertyTypes(PageDefinition pageDefinition, PropertyDefinition propertyDefinition)
        {
            if (pageDefinition.Type.TypeName == ComposerPropertyTypes.ContentArea)
            {
                propertyDefinition.DisplayEditUI = true;
                propertyDefinition.Searchable = false;
                propertyDefinition.LanguageSpecific = true;
            }
        }

    }
}
