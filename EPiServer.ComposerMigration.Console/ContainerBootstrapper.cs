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
using EPiServer.ComposerMigration.CodeGeneration;
using Microsoft.CSharp;
using StructureMap;
using System.CodeDom.Compiler;

namespace EPiServer.ComposerMigration
{
    static class ContainerBootstrapper
    {
        public static void Bootstrap()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<IPropertyMapper>().Use<PropertyMapper>();
                x.For<IContentTypeMapper>().Use<ContentTypeMapper>();
                x.For<IContentTypeCodeBuilder>().Use<ContentTypeCodeBuilder>();
                x.For<IContentTypeWriter>().Use<ContentTypeWriter>();
                x.For<IXmlElementParser>().Use<PageTypeElementParser>();
                x.For<CodeDomProvider>().Use(() => new CSharpCodeProvider());
                x.For<IPackageReaderContext>().Use( () => (IPackageReaderContext)null);
            });
        }
    }

}
