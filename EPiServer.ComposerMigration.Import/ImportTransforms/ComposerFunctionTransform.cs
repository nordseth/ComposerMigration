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
	public class ComposerFunctionTransform : IImportTransform
	{
		private readonly IContentMap _contentMap;
		private readonly ExportLinkResolver _exportLinkResolver;
		private readonly ComposerSerializer _composerSerializer;
		private readonly ICollection<string> _possibleFunctionNameProperties;
		private readonly bool _includeUnusedBlocks;

		public ComposerFunctionTransform(IContentMap contentMap, ComposerSerializer composerSerializer, ExportLinkResolver exportLinkResolver, IComposerImportOptions options)
		{
			_contentMap = contentMap;
			_composerSerializer = composerSerializer;
			_exportLinkResolver = exportLinkResolver;

			options = options ?? ComposerImportOptions.Default;
			_possibleFunctionNameProperties = new HashSet<string>(options.BlockNameProperties ?? Enumerable.Empty<string>());
			_includeUnusedBlocks = options.IncludeUnusedBlocks;
		}

		public virtual bool Transform(ITransferContentData transferContentData)
		{
			var rawContent = transferContentData.RawContentData;

			var functionProperty = rawContent.GetProperty(ComposerProperties.ContentFunction);
			if (functionProperty == null)
			{
				return true;
			}

			// Deserialize the Extension Function Struct
			var functionInfo = _composerSerializer.Deserialize<ComposerContentFunctionInformation>(functionProperty.Value);

			var isGlobalBlock = functionInfo != null && functionInfo.Type == ComposerContentFunctionCategory.Global;

			var guid = transferContentData.RawContentData.PageGuid();
			var parent = _contentMap.GetParentPage(guid);

			// If a (non-global) block doesn't have a parent, it's an orphaned block and we won't import it
			if (parent == null && !_includeUnusedBlocks && !isGlobalBlock)
			{
				return false;
			}

			// Move all Global and Unused blocks to the GlobalBlockFolder
			if (isGlobalBlock || parent == null)
			{
				rawContent.SetPropertyValue(MetaDataProperties.PageParentLink, _exportLinkResolver.GetExportableLink(ContentReference.GlobalBlockFolder));
			}

			// Update the PageName to a more friendly name
			var property = rawContent.GetProperty(MetaDataProperties.PageName);
			if (property != null)
			{
				var friendlyName = CreateFriendlyName(rawContent, functionInfo);
				if (!string.IsNullOrEmpty(friendlyName))
				{
					property.Value = friendlyName;
				}
			}

			// Don't change language of Global Blocks
			if (!isGlobalBlock && parent != null)
			{
				rawContent.Language(parent.Language);
				rawContent.MasterLanguage(parent.Language);
			}

			return true;
		}

		public virtual string CreateFriendlyName(RawContent content, ComposerContentFunctionInformation functionInfo)
		{
			if (functionInfo != null && !string.IsNullOrEmpty(functionInfo.Name))
			{
				return functionInfo.Name;
			}

			string friendlyName = content.PageType();

			if (_possibleFunctionNameProperties.Count > 0)
			{
				var headingProperty = content.Property.FirstOrDefault(p => _possibleFunctionNameProperties.Contains(p.Name));

				if (headingProperty != null && !headingProperty.IsNull)
				{
					friendlyName += " - " + headingProperty.Value;
				}
			}

			return friendlyName;
		}

	}
}
