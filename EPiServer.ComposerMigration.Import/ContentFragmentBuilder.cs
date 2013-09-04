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
using EPiServer.Core.Html.StringParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration
{
    public class ContentFragmentBuilder
    {
        private readonly ContentFragmentFactory _fragmentFactory;
        private readonly ISecuredFragmentMarkupGeneratorFactory _markupGeneratorFactory;

        public ContentFragmentBuilder(ContentFragmentFactory fragmentFactory, ISecuredFragmentMarkupGeneratorFactory markupGeneratorFactory)
        {
            _fragmentFactory = fragmentFactory;
            _markupGeneratorFactory = markupGeneratorFactory;
        }

        public virtual ContentFragment CreateFragment(Guid contentGuid, string personalizationGroup, IEnumerable<Guid> visitorGroups)
        {
            var markupGenerator = _markupGeneratorFactory.CreateSecuredFragmentMarkupGenerator();
            markupGenerator.ContentGroup = personalizationGroup;
            markupGenerator.RoleSecurityDescriptor.RoleIdentities = visitorGroups.Select(g => g.ToString());
            return _fragmentFactory.CreateContentFragment(null, contentGuid, markupGenerator);
        }

    }
}
