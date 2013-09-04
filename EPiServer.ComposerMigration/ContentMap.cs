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
    public class ContentMap : IContentMap
    {
        private static readonly Guid DefaultContentGuid = Guid.Empty;
        private static readonly DateTime JsReferenceDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly ILookup<string, ComposerContentFunction> EmptyContentFunctionLookup = Enumerable.Empty<ComposerContentFunction>().ToLookup(x => (string)null);

        private readonly Dictionary<Tuple<Guid, string>, IComposerPage> _contentStorage = new Dictionary<Tuple<Guid, string>, IComposerPage>();
        private readonly Dictionary<Tuple<Guid, string>, IComposerContent> _normalizedContentStorage = new Dictionary<Tuple<Guid, string>, IComposerContent>();
        private readonly HashSet<Guid> _personalizationContainers = new HashSet<Guid>();
        private readonly List<Tuple<Guid, Guid>> _visitorGroupMap = new List<Tuple<Guid, Guid>>();
        private readonly Dictionary<Guid, IComposerPage> _parentPages = new Dictionary<Guid, IComposerPage>();
        private ILookup<Guid, Guid> _visitorGroupLookup;
        private bool _isRestructured;

        public virtual int PageCount
        {
            get { return _contentStorage.Count; }
        }

        public virtual void AddPage(IComposerPage page)
        {
            var key = Tuple.Create(page.Guid, page.Language);
            _contentStorage[key] = page;
            _normalizedContentStorage[key] = page;

            var descendentFunctions = GetDecendentFunctions(page).ToArray();

            foreach (var function in descendentFunctions)
            {
                // Set language on function
                function.Language = page.Language;

                // Add to normalized storage
                key = Tuple.Create(function.Guid, function.Language);
                _normalizedContentStorage[key] = function;

                // Add to reverse lookup
                _parentPages[function.Guid] = page;
            }

            if (page.ShadowGuid != Guid.Empty && descendentFunctions.Any())
            {
                // Add shadow containers to reverse lookup
                _parentPages.Add(page.ShadowGuid, page);
            }

            _isRestructured = false;
        }

        public virtual IComposerPage GetPage(Guid guid, string language)
        {
            var key = Tuple.Create(guid, language);
            IComposerPage page;
            _contentStorage.TryGetValue(key, out page);
            return page;
        }

        public virtual ILookup<string, ComposerContentFunction> GetContentFunctions(Guid guid, string language)
        {
            var key = Tuple.Create(guid, language);

            IComposerContent content;
            if (!_normalizedContentStorage.TryGetValue(key, out content))
            {
                var parent = GetParentPage(guid);
                if (parent == null)
                {
                    return EmptyContentFunctionLookup;
                }

                key = Tuple.Create(guid, parent.Language);
                if (!_normalizedContentStorage.TryGetValue(key, out content))
                {
                    return EmptyContentFunctionLookup;
                }
            }

            if (!_isRestructured)
            {
                RestructurePersonalization();
            }

            var functions = from area in content.ContentAreas
                            from func in area.ContentFunctions
                            select new { Name = area.Name, ContentFunction = func };

            return functions.ToLookup(x => x.Name, x => x.ContentFunction, StringComparer.OrdinalIgnoreCase);
        }

        public virtual IComposerPage GetParentPage(Guid guid)
        {
            IComposerPage page;
            _parentPages.TryGetValue(guid, out page);
            return page;
        }

        public virtual void AddPersonalisationContainer(Guid guid)
        {
            _personalizationContainers.Add(guid);
        }

        public virtual void AddVisitorGroupMap(Guid groupReference, params Guid[] visitorGroups)
        {
            if (groupReference == Guid.Empty)
            {
                return;
            }

            if (visitorGroups != null && visitorGroups.Any())
            {
                _visitorGroupMap.AddRange(visitorGroups.Select(vg => Tuple.Create(groupReference, vg)));
            }
            else
            {
                // If no visitor groups are assigned this indicates that the ContentGroup is the default content
                _visitorGroupMap.Add(Tuple.Create(groupReference, DefaultContentGuid));
            }
            // Clear cached lookup
            _visitorGroupLookup = null;
        }

        public virtual IEnumerable<Guid> GetVisitorGroups(Guid reference)
        {
            if (_visitorGroupLookup == null)
            {
                _visitorGroupLookup = _visitorGroupMap.ToLookup(x => x.Item1, y => y.Item2);
            }
            return _visitorGroupLookup[reference];
        }

        public virtual void RestructurePersonalization()
        {
            var personalized =
                (from content in _contentStorage.Values
                 from area in content.ContentAreas
                 from function in area.ContentFunctions
                 where _personalizationContainers.Contains(function.Guid)
                 select new { Area = area, PersonalizationContainer = function }).ToArray();

            foreach (var item in personalized)
            {
                RestructurePersonalizationContainer(item.PersonalizationContainer, item.Area);
            }

            _isRestructured = true;
        }

        public virtual void RestructurePersonalizationContainer(ComposerContentFunction container, ComposerContentArea parentArea)
        {
            var parentFunctions = parentArea.ContentFunctions;
            var insertIndex = parentFunctions.IndexOf(container);
            parentFunctions.RemoveAt(insertIndex);

            // TODO: This assumption should be validated sometime before this call!
            // Composer Personalization Containers should only have one single ContentArea
            var functions = container.ContentAreas.Single().ContentFunctions;
            var personalizationGroup = CreatePersonalizationGroupName();
            foreach (var f in functions)
            {
                f.PersonalizationGroup = personalizationGroup;
                ExpandVisitorGroupReference(f);
                parentFunctions.Insert(insertIndex++, f);
            }
        }

        public virtual void ExpandVisitorGroupReference(ComposerContentFunction function)
        {
            if (function.VisitorGroupContainerID == Guid.Empty)
                return;

            function.VisitorGroups = GetVisitorGroups(function.VisitorGroupContainerID).Where(x => x != Guid.Empty).ToArray();
        }

        protected virtual IEnumerable<IComposerContent> GetDecendentFunctions(IEnumerable<IComposerContent> contentList)
        {
            return contentList.SelectMany(GetDecendentFunctions);
        }

        protected virtual IEnumerable<IComposerContent> GetDecendentFunctions(IComposerContent content)
        {
            var functions = from area in content.ContentAreas
                            from func in area.ContentFunctions
                            select func;

            foreach (var f in functions)
            {
                yield return f;
                // Call recursive to look for further nesting 
                foreach (var decendent in GetDecendentFunctions(f))
                {
                    yield return decendent;
                }
            }
        }

        private static double JsTicks
        {
            get { return Math.Truncate(DateTime.UtcNow.Subtract(JsReferenceDate).TotalMilliseconds); }
        }

        private static string CreatePersonalizationGroupName()
        {
            return "group_" + JsTicks.ToString("F0");
        }

    }

}
