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
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.ComposerMigration
{
    public interface IContentMap
    {
        /// <summary>
        /// Gives an indication on how many pages that has been added to the map
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Adds a composer content item to the map for later lookup.
        /// </summary>
        /// <param name="page">The content item to add.</param>
        void AddPage(IComposerPage page);

        /// <summary>
        /// Adds a personalization container to the content map.
        /// </summary>
        void AddPersonalisationContainer(Guid guid);

        /// <summary>
        /// Add the mapping of visitor groups to the content map.
        /// </summary>
        /// <param name="groupReference"></param>
        /// <param name="visitorGroups"></param>
        void AddVisitorGroupMap(Guid groupReference, params Guid[] visitorGroups);
        
        /// <summary>
        /// Retrieves a lookup list of all functions of a specific content item, keyed on content area name.
        /// If no page or function with the provided GUID/Language pair has been registered, an empty lookup is returned.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method will not retrieve any functions in nested content areas of the provided item.
        /// </remarks>
        ILookup<string, ComposerContentFunction> GetContentFunctions(Guid guid, string language);

        /// <summary>
        /// Get the parent Composer page that contains a specific Composer content function or container.
        /// If the content item has not been added to the map, null is returned.
        /// </summary>
        /// <param name="guid">Unique identifier of the content function or container.</param>
        /// <returns>The parent page if found; otherwise null.</returns>
        IComposerPage GetParentPage(Guid guid);
    }
}
