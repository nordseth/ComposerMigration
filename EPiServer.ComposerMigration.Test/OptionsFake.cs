using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration
{
    internal class OptionsFake : IComposerTranformationOptions, ICodeGenerationOptions
    {
        public OptionsFake()
        {
            OutputDirectory = "output";
            Namespace = "MyNamespace";
            PageSuffix = "Page";
            PageBaseClass = "EPiServer.Core.PageData";
            BlockSuffix = "Block";
            BlockBaseClass = "EPiServer.Core.BlockData";
            Imports = Enumerable.Empty<string>();
            IncludeGroupNameInNamespace = true;
        }

        public string OutputDirectory { get; set; }
        public string Namespace { get; set; }

        public string PageSuffix { get; set; }
        public string PageBaseClass { get; set; }
        public string PageNamespace { get; set; }

        public string BlockBaseClass { get; set; }
        public string BlockSuffix { get; set; }
        public string BlockNamespace { get; set; }

        public IEnumerable<string> Imports { get; set; }

        public bool IncludeUnsupportedProperties { get; set; }
        public bool ForceLegacyPropertyWrapper { get; set; }
        public bool UseNullableValueTypes { get; set; }
        public bool UseEditable { get; set; }
        public bool IncludeGroupNameInNamespace { get; set; }        
    }
}
