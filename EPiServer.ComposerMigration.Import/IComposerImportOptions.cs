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
    /// <summary>
    /// Defines the options for how an import should be done
    /// </summary>
    public interface IComposerImportOptions
    {
        bool Enabled { get; }
        bool TransformPageTypeIdentifiers { get; }
        bool IncludeUnusedBlocks { get; }
        IEnumerable<string> BlockNameProperties { get; }
    }

    internal class ComposerImportOptions : IComposerImportOptions
    {
        private static readonly string[] FunctionNames = new[] { "Heading", "Header", "Headline", "Title", "Name" };
        private ComposerImportOptions() { }

        public static readonly IComposerImportOptions Default = new ComposerImportOptions();

        public bool Enabled
        {
            get { return true; }
        }

        public bool TransformPageTypeIdentifiers
        {
            get { return true; }
        }

        public bool IncludeUnusedBlocks
        {
            get { return false; }
        }

        public IEnumerable<string> BlockNameProperties
        {
            get { return FunctionNames; }
        }

    }
}
