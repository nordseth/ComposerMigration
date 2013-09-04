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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration
{
    public class IgnoredContentTransform : IImportTransform
    {
        private readonly IContentMap _contentMap;

        private static readonly HashSet<string> SystemPageTypes = new HashSet<string> { "SysRoot", "SysRecycleBin", ComposerPageTypes.GlobalFunctionEditPage, ComposerPageTypes.Prefix + ComposerPageTypes.GlobalFunctionEditPage };

        public IgnoredContentTransform(IContentMap contentMap)
        {
            _contentMap = contentMap;
        }

        public virtual bool Transform(ITransferContentData transferContentData)
        {
            var pageType = transferContentData.RawContentData.PageType();

            // Don't import certain system page types
            if (SystemPageTypes.Contains(pageType))
            {
                return false;
            }

            // Don't import Personalization Containers
            if (transferContentData.RawContentData.GetProperty(ComposerProperties.PersonalizationData) != null)
            {
                return false;
            }

            // Don't import empty ComposerContainers
            if (transferContentData.IsComposerContainer())
            {
                var parent = _contentMap.GetParentPage(transferContentData.RawContentData.PageGuid());
                if (parent == null)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
