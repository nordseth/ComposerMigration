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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration
{
    public class ComposerMigrationOptions : ICodeGenerationOptions
    {
        private ComposerMigrationOptions() { }

        public static readonly ComposerMigrationOptions Default = new ComposerMigrationOptions();

        public string OutputDirectory
        {
            get { return "output"; }
        }
        public string Namespace
        {
            get { return "MyNamespace"; }
        }
        public string PageSuffix
        {
            get { return "Page"; }
        }
        public string PageBaseClass
        {
            get { return "EPiServer.Core.PageData"; }
        }
        public string PageNamespace
        {
            get { return "Pages"; }
        }
        public string BlockBaseClass
        {
            get { return "EPiServer.Core.BlockData"; }
        }
        public string BlockSuffix
        {
            get { return "Block"; }
        }
        public string BlockNamespace
        {
            get { return "Blocks"; }
        }
        public IEnumerable<string> Imports
        {
            get { return Enumerable.Empty<string>(); }
        }
        public bool ForceLegacyPropertyWrapper
        {
            get { return false; }
        }
        public bool UseNullableValueTypes
        {
            get { return false; }
        }
        public bool UseEditable
        {
            get { return false; }
        }
        public bool IncludeGroupNameInNamespace
        {
            get { return true; }
        }
        
    }
}
