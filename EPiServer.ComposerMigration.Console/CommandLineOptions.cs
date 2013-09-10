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
using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace EPiServer.ComposerMigration
{
    class CommandLineOptions : ICodeGenerationOptions
    {
        [Option('s', "source", DefaultValue = "ExportedData.episerverdata", HelpText = "Path to the source package (.episerverdata) that model classes are generated from. Can be relative or absolute.")]
        public string SourcePackage { get; set; }

        [Option('o', "output", DefaultValue = "output", HelpText = "Path to the folder where model class files should be generated. Can be relative or absolute.")]
        public string OutputDirectory { get; set; }

        [Option('n', "namespace", DefaultValue = "MySolution.Models", HelpText = "Base namespace that all generated model classes should be placed in.")]
        public string Namespace { get; set; }

        [Option("page-namespace", DefaultValue = "Pages", HelpText = "Namespace where page type classes will be placed.")]
        public string PageNamespace { get; set; }

        [Option("page-suffix", DefaultValue = "Page", HelpText = "Suffix that all page type class names must end with.")]
        public string PageSuffix { get; set; }

        [Option('p', "page-base-class", DefaultValue = "EPiServer.Core.PageData", HelpText = "Base class that all page type classes will inherit from.")]
        public string PageBaseClass { get; set; }

        [Option("block-namespace", DefaultValue = "Blocks", HelpText = "Namespace where block type classes will be placed.")]
        public string BlockNamespace { get; set; }

        [Option("block-suffix", DefaultValue = "Block", HelpText = "Suffix that all block type class names must end with.")]
        public string BlockSuffix { get; set; }

        [Option('b', "block-base-class", DefaultValue = "EPiServer.Core.BlockData", HelpText = "Base class that all block type classes will inherit from.")]
        public string BlockBaseClass { get; set; }

        [OptionArray('i', "imports", HelpText = "Namespaces that should be imported (using) in all classes. Note that some namespaces are added automatically if needed.")]
        public string[] Imports { get; set; }

        [Option('w', "legacy-wrapper", DefaultValue = false, HelpText = "Indication if properties of unknown types should get a UIHint to force a legacy property wrapper to appear.")]
        public bool ForceLegacyPropertyWrapper { get; set; }

        [Option('u', "nullable", DefaultValue = false, HelpText = "Indication if properties using value types should use the nullable version of the type.")]
        public bool UseNullableValueTypes { get; set; }

        [Option('g', "group-namespaces", DefaultValue = false, HelpText = "Indication if content types that has a group name prefix should include this name as a namespace.")]
        public bool IncludeGroupNameInNamespace { get; set; }

        [Option("editable", DefaultValue = false, HelpText = "Indication if properties not visible in edit mode should be generated using the Editable attribute rather than with ScaffoldColumn.")]
        public bool UseEditable { get; set; }

        [Option("verbose", DefaultValue = false, HelpText = "Include this to get extended information in the console.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        IEnumerable<string> ICodeGenerationOptions.Imports
        {
            get { return this.Imports; }
        }

    }
}
