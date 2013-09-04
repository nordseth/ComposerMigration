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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration.CodeGeneration
{
    public class ContentTypeCodeBuilder : IContentTypeCodeBuilder
    {
        private static readonly IEnumerable<string> StandardNamespaces = new[] { 
            "System", // Needed for some Nullable types
            "EPiServer.DataAnnotations", // Used by attributes
            "System.ComponentModel.DataAnnotations", // Used by attributes
            "EPiServer.DataAbstraction", // Used by SystemTabNames constant 
            "EPiServer.Web", // Used by UIHint constants
        };
        public static readonly int GlobalNamespaceIndex = 0;
        public static readonly int ClassNamespaceIndex = 1;

        private readonly ICodeGenerationOptions _options;
        private readonly CodeDomProvider _codeProvider;

        private CodeNamespace _globalNameSpace;

        public ContentTypeCodeBuilder(CodeDomProvider codeProvider, ICodeGenerationOptions options)
        {
            _codeProvider = codeProvider ?? new CSharpCodeProvider();
            _options = options ?? (ICodeGenerationOptions)ComposerMigrationOptions.Default;
        }

        public virtual CodeCompileUnit BuildContentTypeClass(ContentType contentType)
        {
            var unit = new CodeCompileUnit();

            // Add global namespace used for Imports
            _globalNameSpace = AddGlobalNamespace(unit);

            // Add main type namespace
            var mainNamespace = AddMainNamespace(unit, contentType);

            // Add main type declaration
            var typeDeclaration = AddTypeDeclaration(mainNamespace, contentType);

            // Add main type attributes
            AddContentTypeAttribute(contentType, typeDeclaration);
            AddAccessAttribute(contentType, typeDeclaration);

            // Add properties
            foreach (var propertyDefinition in contentType.PropertyDefinitions)
            {
                AddPropertyDeclaration(propertyDefinition, typeDeclaration);
            }

            AddDefaultValuesMethod(contentType, typeDeclaration);

            return unit;
        }

        protected virtual CodeNamespace AddMainNamespace(CodeCompileUnit unit, ContentType contentType)
        {
            var ns = _options.Namespace;

            if (string.IsNullOrEmpty(ns))
            {
                ns = ComposerMigrationOptions.Default.Namespace;
            }

            if (!string.IsNullOrEmpty(contentType.Namespace))
            {
                ns += "." + contentType.Namespace;
            }

            var mainNamespace = new CodeNamespace(ns);
            unit.Namespaces.Add(mainNamespace);
            return mainNamespace;
        }

        protected virtual CodeNamespace AddGlobalNamespace(CodeCompileUnit unit)
        {
            var globalNs = new CodeNamespace();
            unit.Namespaces.Add(globalNs);

            var imports = _options.Imports != null ? StandardNamespaces.Concat(_options.Imports) : StandardNamespaces;

            foreach (var import in imports)
            {
                globalNs.Imports.Add(new CodeNamespaceImport(import));
            }

            return globalNs;
        }

        protected virtual CodeTypeDeclaration AddTypeDeclaration(CodeNamespace classNamespace, ContentType contentType)
        {
            // One last check that the name is good, e.g. not a reserved word or begins with a number
            var typeName = _codeProvider.CreateEscapedIdentifier(contentType.Name);

            var typeDeclaration = new CodeTypeDeclaration(typeName);
            classNamespace.Types.Add(typeDeclaration);

            // Add base type
            string baseTypeName;
            switch (contentType.ContentTypeCategory)
            {
                case ContentTypeCategory.Page:
                    baseTypeName = _options.PageBaseClass;
                    break;
                case ContentTypeCategory.Block:
                    baseTypeName = _options.BlockBaseClass;
                    break;
                case ContentTypeCategory.Undefined:
                case ContentTypeCategory.BlockFolder:
                default:
                    baseTypeName = null;
                    break;
            }

            if (!string.IsNullOrEmpty(baseTypeName))
            {
                typeDeclaration.BaseTypes.Add(new CodeTypeReference(SimplifyTypeName(baseTypeName)));
            }

            return typeDeclaration;
        }

        protected virtual void AddContentTypeAttribute(ContentType contentType, CodeTypeDeclaration typeDeclaration)
        {
            var contentTypeAttribute = new CodeAttributeDeclaration("ContentType");

            if (contentType.GUID != Guid.Empty)
            {
                contentTypeAttribute.AddArgument("GUID", contentType.GUID.ToString());
            }
            if (!string.IsNullOrWhiteSpace(contentType.DisplayName))
            {
                contentTypeAttribute.AddArgument("DisplayName", contentType.DisplayName);
            }
            if (contentType.SortOrder != 0)
            {
                contentTypeAttribute.AddArgument("Order", contentType.SortOrder);
            }
            if (!contentType.IsAvailable)
            {
                contentTypeAttribute.AddArgument("AvailableInEditMode", contentType.IsAvailable);
            }
            if (!string.IsNullOrWhiteSpace(contentType.Description))
            {
                contentTypeAttribute.AddArgument("Description", contentType.Description);
            }
            // We will not add the group name as it is included in the DisplayName
            //if (!string.IsNullOrWhiteSpace(contentType.GroupName))
            //{
            //    contentTypeAttribute.AddArgument("GroupName", contentType.GroupName);
            //}

            typeDeclaration.CustomAttributes.Add(contentTypeAttribute);
        }

        protected virtual void AddAccessAttribute(ContentType contentType, CodeTypeDeclaration typeDeclaration)
        {
            if (contentType.ACL == null)
                return;

            var accessEntries = contentType.ACL.Entries.Where(acl => !acl.Name.Equals("Everyone"));

            if (!accessEntries.Any())
                return;

            var accessAttribute = new CodeAttributeDeclaration("Access");

            var users = string.Join(", ", accessEntries.Where(acl => acl.EntityType == SecurityEntityType.User).Select(acl => acl.Name));
            if (!string.IsNullOrEmpty(users))
            {
                accessAttribute.AddArgument("Users", users);
            }

            var roles = string.Join(", ", accessEntries.Where(acl => acl.EntityType == SecurityEntityType.Role).Select(acl => acl.Name));
            if (!string.IsNullOrEmpty(users))
            {
                accessAttribute.AddArgument("Roles", roles);
            }

            typeDeclaration.CustomAttributes.Add(accessAttribute);
        }

        protected virtual void AddPropertyDeclaration(PropertyDefinition propertyDefinition, CodeTypeDeclaration typeDeclaration)
        {
            var propertyType = propertyDefinition.Type;
            propertyType.TypeName = SimplifyTypeName(propertyDefinition.Type.TypeName);

            // This uses a bit of an ugly hack with a field plus a name as auto-properties are not supported by CodeDom :(
            var property = new CodeMemberField
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Const, // use const as a marker to be replaced by virtual by FilterWriter
                Name = propertyDefinition.Name + " { get; set; }", // Add auto get & set as part of the name to the generator
                Type = propertyType.Type != null ? new CodeTypeReference(propertyType.Type) : new CodeTypeReference(propertyType.TypeName)
            };

            typeDeclaration.Members.Add(property);

            // Add backing type attribute if needed. 
            if (!string.IsNullOrEmpty(propertyType.BackingTypeName))
            {
                propertyType.BackingTypeName = SimplifyTypeName(propertyType.BackingTypeName);
                property.AddCustomAttribute("BackingType", string.Format("$typeof({0})", propertyType.BackingTypeName));
            }

            // Add UIHint attribute for Images and Documents
            if (!string.IsNullOrEmpty(propertyType.UIHint))
            {
                property.AddCustomAttribute("UIHint", propertyType.UIHint);
            }

            // Editable attribute
            if (!propertyDefinition.DisplayEditUI)
            {
                if (_options.UseEditable)
                {
                    property.AddCustomAttribute("Editable", false);
                }
                else
                {
                    property.AddCustomAttribute("ScaffoldColumn", false);
                }
            }

            // Searchable attribute
            if (propertyDefinition.Searchable ^ IsStringType(propertyType))
            {
                property.AddCustomAttribute("Searchable", propertyDefinition.Searchable);
            }

            // Searchable attribute
            if (propertyDefinition.LanguageSpecific ^ IsStringType(propertyType))
            {
                property.AddCustomAttribute("CultureSpecific", propertyDefinition.LanguageSpecific);
            }

            // Required attribute
            if (propertyDefinition.Required)
            {
                property.AddCustomAttribute("Required");
            }

            // Display attribute
            var displayAttr = property.AddCustomAttribute("Display");
            if (!string.IsNullOrEmpty(propertyDefinition.EditCaption) && propertyDefinition.EditCaption != propertyDefinition.Name)
            {
                displayAttr.AddArgument("Name", propertyDefinition.EditCaption);
            }
            if (!string.IsNullOrEmpty(propertyDefinition.HelpText))
            {
                displayAttr.AddArgument("Description", propertyDefinition.HelpText);
            }
            if (!string.IsNullOrEmpty(propertyDefinition.GroupName))
            {
                displayAttr.AddArgument("GroupName", propertyDefinition.GroupName);
            }
            if (propertyDefinition.FieldOrder != 0)
            {
                displayAttr.AddArgument("Order", propertyDefinition.FieldOrder);
            }
        }

        protected virtual void AddDefaultValuesMethod(ContentType contentType, CodeTypeDeclaration typeDeclaration)
        {
            if (contentType.ContentTypeCategory != ContentTypeCategory.Page || contentType.Defaults == null)
            {
                return;
            }

            var method = new CodeMemberMethod
            {
                Name = "SetDefaultValues",
                Attributes = MemberAttributes.Public | MemberAttributes.Override,
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression("ContentType", "contentType"));

            // base.SetDefaultValues()
            method.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "SetDefaultValues", new CodeArgumentReferenceExpression("contentType")));

            var defaults = contentType.Defaults;

            if (!defaults.VisibleInMenu)
            {
                // VisibleInMenu = false;
                method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression { PropertyName = "VisibleInMenu" }, new CodePrimitiveExpression(false)));
            }

            if (defaults.ChildOrderRule != FilterSortOrder.None && defaults.ChildOrderRule != FilterSortOrder.CreatedDescending)
            {
                // this[MetaDataProperties.PageChildOrderRule] = FilterSortOrder.XX;
                method.Statements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeThisReferenceExpression(), new CodeSnippetExpression("MetaDataProperties.PageChildOrderRule")), new CodeSnippetExpression("FilterSortOrder." + defaults.ChildOrderRule)));
                RegisterImport("EPiServer.Filters");
            }

            if (defaults.PeerOrder >= 0)
            {
                // this[MetaDataProperties.PagePeerOrder] = 100;
                method.Statements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeThisReferenceExpression(), new CodeSnippetExpression("MetaDataProperties.PagePeerOrder")), new CodePrimitiveExpression(defaults.PeerOrder)));
            }

            if (!string.IsNullOrEmpty(defaults.DefaultPageName))
            {
                // PageName = "The default name";
                method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression { PropertyName = "PageName" }, new CodePrimitiveExpression(defaults.DefaultPageName)));
            }

            // Start/Stop Publishing Offsets are not exported currently due to a bug
            //if (defaults.StartPublishOffset != TimeSpan.Zero)
            //{
            //    // DateTime.Now
            //    var datetimeNow = new CodePropertyReferenceExpression(new CodePrimitiveExpression(typeof(DateTime)), "Now");
            //    // TimeSpan.FromTicks
            //    var fromTicks = new CodeMethodInvokeExpression(new CodePrimitiveExpression(typeof(TimeSpan)), "FromTicks", new CodePrimitiveExpression(defaults.StartPublishOffset));

            //    // StartPublish = DateTime.Now.Add(TimeSpan.FromTicks(6511898334586));
            //    method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression { PropertyName = "StartPublish" }, new CodeMethodInvokeExpression(datetimeNow, "Add", fromTicks)));
            //}

            if (method.Statements.Count > 1)
            {
                typeDeclaration.Members.Add(method);
            }
        }

        private bool IsStringType(PropertyDefinitionType propertyType)
        {
            return propertyType.Type == typeof(string) || propertyType.TypeName == "XhtmlString";
        }

        private string SimplifyTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return typeName;
            }

            var dot = typeName.LastIndexOf('.');

            if (dot < 0)
            {
                return typeName;
            }

            var ns = typeName.Substring(0, dot);
            RegisterImport(ns);

            return typeName.Substring(dot + 1);
        }

        private void RegisterImport(string ns)
        {
            _globalNameSpace.Imports.Add(new CodeNamespaceImport(ns));
        }
    }
}
