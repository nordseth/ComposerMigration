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
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.Core.Transfer;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration
{
    public class DynamicFunctionTransform : IImportTransform
    {
        private readonly ExportLinkResolver _exportLinkResolver;
        private readonly ComposerSerializer _composerSerializer;

        public DynamicFunctionTransform(ComposerSerializer composerSerializer, ExportLinkResolver exportLinkResolver)
        {
            _composerSerializer = composerSerializer;
            _exportLinkResolver = exportLinkResolver;
        }

        public virtual bool Transform(ITransferContentData transferContentData)
        {
            if (transferContentData.IsComposerFunction())
            {
                transferContentData.ForAllContent(TransformDynamicFunction);
            }
            return true;
        }

        public virtual void TransformDynamicFunction(RawContent rawContent)
        {
            var functionPickerProperties = rawContent.Property.Where(x => x.TypeName == ComposerPropertyTypes.FunctionPicker).ToArray();

            // Transform each FunctionPicker into a ContentReference property
            foreach (var functionPickerProperty in functionPickerProperties)
            {
                TransformFunctionPickerProperty(functionPickerProperty);
            }

        }

        public virtual void TransformFunctionPickerProperty(RawProperty functionPickerProperty)
        {
            // Change type to ContentReference
            functionPickerProperty.Type = PropertyDataType.ContentReference;
            functionPickerProperty.TypeName = null;
            functionPickerProperty.AssemblyName = null;

            var data = _composerSerializer.Deserialize<FunctionPickerData>(functionPickerProperty.Value);

            // Convert the Value to an ExportableLink
            if (data != null && data.FunctionLink != null)
            {
                functionPickerProperty.Value = _exportLinkResolver.GetExportableLink(data.FunctionLink.Guid, data.PageLanguage);
                functionPickerProperty.IsNull = string.IsNullOrEmpty(functionPickerProperty.Value);
            }
            else
            {
                functionPickerProperty.IsNull = true;
            }
        }

    }
}
