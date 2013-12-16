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
using EPiServer.Core.Transfer;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration
{
	public class ComposerContainerTransform : IImportTransform
	{
		private readonly IContentMap _contentMap;
		private readonly ExportLinkResolver _exportLinkResolver;
		private readonly bool _includeUnusedBlocks;

		public ComposerContainerTransform(IContentMap contentMap, ExportLinkResolver exportLinkResolver, IComposerImportOptions options)
		{
			_contentMap = contentMap;
			_exportLinkResolver = exportLinkResolver;
			options = options ?? ComposerImportOptions.Default;
			_includeUnusedBlocks = options.IncludeUnusedBlocks;
		}

		public virtual bool Transform(ITransferContentData transferContentData)
		{
			if (!transferContentData.IsComposerContainer())
			{
				return true;
			}

			var parent = _contentMap.GetParentPage(transferContentData.RawContentData.PageGuid());

			if (parent == null && !_includeUnusedBlocks)
			{
				return false;
			}

			transferContentData.ForAllContent(c => TransformContainer(c, parent));

			return true;
		}

		public virtual void TransformContainer(RawContent rawContent, IComposerPage parent)
		{
			if (rawContent == null)
				throw new ArgumentNullException("rawContent");

			rawContent.SetPropertyValue(MetaDataProperties.PageParentLink, _exportLinkResolver.GetExportableLink(new ContentReference(4))); // id of the SysContentAssets folder
			rawContent.SetPropertyValue(MetaDataProperties.PageContentOwnerID, parent.Guid.ToString());

			if (parent != null)
			{
				rawContent.PageName(parent.Name);
				rawContent.Language(parent.Language);
				rawContent.MasterLanguage(parent.Language);
			}
		}

	}
}
