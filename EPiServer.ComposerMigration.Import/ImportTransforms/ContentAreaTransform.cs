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
using EPiServer.SpecializedProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using EPiServer.DataAbstraction;

namespace EPiServer.ComposerMigration
{
	public class ContentAreaTransform : IImportTransform
	{
		private readonly IContentMap _contentMap;
		private readonly ContentFragmentBuilder _contentFragmentBuilder;
		private readonly IContentTransferContext _transferContext;

		public ContentAreaTransform(IContentMap contentMap, ContentFragmentBuilder contentFragmentBuilder, IContentTransferContext transferContext)
		{
			_contentMap = contentMap;
			_contentFragmentBuilder = contentFragmentBuilder;
			_transferContext = transferContext;
		}

		public virtual bool Transform(ITransferContentData transferContentData)
		{
			if (transferContentData.IsComposerContent())
			{
				transferContentData.ForAllContent(TransformContentAreas);
			}
			return true;
		}

		public virtual void TransformContentAreas(RawContent rawContent)
		{
			var functions = _contentMap.GetContentFunctions(rawContent.PageGuid(), rawContent.Language());
			var cmsContentAreaTypeName = typeof(PropertyContentArea).FullName;

			IComposerPage page = null;
			bool contentFunction = rawContent.GetProperty(ComposerProperties.ContentFunction) != null;

			foreach (var contentAreaProperty in rawContent.Property.Where(p => p.TypeName == ComposerPropertyTypes.ContentArea))
			{
				// Change TypeName to CMS Content Area
				contentAreaProperty.TypeName = cmsContentAreaTypeName;

				// Add blocks to Content Areas
				PopulateContentArea(contentAreaProperty, functions[contentAreaProperty.Name]);

				if (!contentFunction)
				{
					page = functions[contentAreaProperty.Name]
						.Where(f => f.Guid != Guid.Empty)
						.Select(f => _contentMap.GetParentPage(f.Guid))
						.FirstOrDefault();
				}
			}

			var pageGuid = rawContent.PageGuid();

			if (page != null)
			{
				Guid shadowPageGuid = page.ShadowGuid;

				rawContent.SetPropertyValue(MetaDataProperties.PageContentAssetsID, shadowPageGuid.ToString());
			}
		}

		public virtual void PopulateContentArea(RawProperty contentAreaProperty, IEnumerable<ComposerContentFunction> composerFunctions)
		{
			composerFunctions = composerFunctions.Where(f => f.Guid != Guid.Empty);

			if (composerFunctions == null || !composerFunctions.Any())
			{
				contentAreaProperty.IsNull = true;
				return;
			}

			// Populate the ContentArea property from the ContentFunction data
			var contentArea = new ContentArea();

			foreach (var block in composerFunctions)
			{
				var mappedGuid = GetMappedGuid(block.Guid);
				var fragment = _contentFragmentBuilder.CreateFragment(mappedGuid, block.PersonalizationGroup, block.VisitorGroups);
				contentArea.Fragments.Add(fragment);
			}

			contentAreaProperty.Value = contentArea.ToInternalString();
			contentAreaProperty.IsNull = false;
		}

		private Guid GetMappedGuid(Guid blockGuid)
		{
			if (_transferContext == null || _transferContext.KeepIdentity || _transferContext.LinkGuidMap == null)
			{
				return blockGuid;
			}

			Guid mappedGuid;
			if (!_transferContext.LinkGuidMap.TryGetValue(blockGuid, out mappedGuid))
			{
				mappedGuid = Guid.NewGuid();
				_transferContext.LinkGuidMap.Add(blockGuid, mappedGuid);
			}
			return mappedGuid;
		}

	}
}
