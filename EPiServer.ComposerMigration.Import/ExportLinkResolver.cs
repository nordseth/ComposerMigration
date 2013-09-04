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
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.ComposerMigration
{
    public class ExportLinkResolver
    {
        private readonly IContentTransferContext _transferContext;

        public ExportLinkResolver(IContentTransferContext transferContext)
        {
            _transferContext = transferContext;
        }

        public virtual string GetExportableLink(ContentReference contentLink)
        {
            var pageMap = PermanentLinkMapStore.Find(contentLink) as PermanentContentLinkMap;
            if (pageMap != null)
            {
                return GetExportableLink(pageMap.Guid, string.Empty);
            }
            return "";
        }

        public virtual string GetExportableLink(Guid guid, string languageBranch)
        {
            return ExportableLink.Create(guid, string.Empty, languageBranch, _transferContext, false).ToString();
        }

    }
}
