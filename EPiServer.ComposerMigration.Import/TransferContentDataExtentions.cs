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
    public static class TransferContentDataExtentions
    {
        /// <summary>
        /// Checks if the current content is either a Composer Page or Composer Function
        /// </summary>
        public static bool IsComposerContent(this ITransferContentData transferContentData)
        {
            return transferContentData.RawContentData.IsComposerContent();
        }

        /// <summary>
        /// Checks if the current content is a composer block.
        /// </summary>
        public static bool IsComposerFunction(this ITransferContentData transferContentData)
        {
            return transferContentData.RawContentData.IsComposerFunction();
        }

        /// <summary>
        /// Checks if the current content is a page that could contain areas and/or blocks.
        /// </summary>
        public static bool IsComposerPage(this ITransferContentData transferContentData)
        {
            return transferContentData.RawContentData.IsComposerPage();
        }

        /// <summary>
        /// Checks if the current content is a Composer Container page
        /// </summary>
        public static bool IsComposerContainer(this ITransferContentData transferContentData)
        {
            return transferContentData.RawContentData.IsComposerContainer();
        }

        public static void ForAllContent(this ITransferContentData transferContentData, Action<RawContent> action)
        {
            action(transferContentData.RawContentData);

            foreach (var languageContent in transferContentData.RawLanguageData)
            {
                action(languageContent);
            }
        }
    }
}
