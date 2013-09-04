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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration
{
    public class PersonalizationContainerCollector : IImportDataCollector
    {
        private readonly IContentMap _contentMap;
        private readonly PersonalizationContainerSerializer _serializer;

        public PersonalizationContainerCollector(IContentMap contentMap, PersonalizationContainerSerializer serializer)
        {
            _contentMap = contentMap;
            _serializer = serializer;
        }

        public virtual void Collect(ITransferContentData transferContentData)
        {
            // We only care about master content as Containers only exist in one language.
            var rawContent = transferContentData.RawContentData;

            // All Personalization Containers has the PersonalizationData property
            var personalizationProperty = rawContent.GetProperty(ComposerProperties.PersonalizationData);

            if (personalizationProperty == null)
            {
                return;
            }

            // Collect GUIDs of all Personalization Containers - even if they are empty!
            _contentMap.AddPersonalisationContainer(rawContent.PageGuid());

            if (personalizationProperty.IsNull)
            {
                return;
            }
            
            // Collect Visitor Group mappings
            foreach (var item in _serializer.Deserialize(personalizationProperty.Value))
            {
                if (item.VisitorGroupList != null && item.VisitorGroupList.Any())
                {
                    _contentMap.AddVisitorGroupMap(item.ContainerID, item.VisitorGroupList.Select(vg => vg.VisitorGroupID).ToArray());
                }
                if (item.IsDefaultContent)
                {
                    _contentMap.AddVisitorGroupMap(item.ContainerID);
                }
            }

        }


    }
}
