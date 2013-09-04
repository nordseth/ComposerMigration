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

namespace EPiServer.ComposerMigration
{
    public class ContentMapCollector : IImportDataCollector
    {
        private readonly IContentMap _contentMap;
        private readonly ComposerSerializer _composerSerializer;

        public ContentMapCollector(IContentMap contentMap, ComposerSerializer composerSerializer)
        {
            _contentMap = contentMap;
            _composerSerializer = composerSerializer;
        }

        public virtual void Collect(ITransferContentData transferContentData)
        {
            if (transferContentData.IsComposerPage())
            {
                transferContentData.ForAllContent(Collect);
            }
        }

        private void Collect(RawContent rawContent)
        {
            IComposerPage composerContent;
            if (TryMap(rawContent, out composerContent))
            {
                _contentMap.AddPage(composerContent);
            }
        }

        public bool TryMap(RawContent rawContent, out IComposerPage composerPage)
        {
            composerPage = null;
            var sourceProperty = rawContent.GetProperty(ComposerProperties.Page);
            if (sourceProperty == null || sourceProperty.IsNull)
            {
                return false;
            }

            // Deserialize the Extension pagestructure
            var specialized = _composerSerializer.Deserialize<ComposerPageInformation>(sourceProperty.Value);

            if (specialized == null || specialized.Page == null)
            {
                return false;
            }

            composerPage = specialized.Page;
            composerPage.Name = rawContent.PageName();
            composerPage.Language = rawContent.Language();
            return true;
        }
    }
}
