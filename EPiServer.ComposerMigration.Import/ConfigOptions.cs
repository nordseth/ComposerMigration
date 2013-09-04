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
using System.Collections.Specialized;
using System.Linq;

namespace EPiServer.ComposerMigration
{
    public class ConfigOptions : IComposerTranformationOptions, IComposerImportOptions
    {
        private const string ConfigKeyPrefix = "ComposerMigration:";

        public ConfigOptions(NameValueCollection appSettings)
        {
            Enabled = GetConfigValue(appSettings, "Enabled", true);
            TransformPageTypeIdentifiers = GetConfigValue(appSettings, "TransformPageTypeIdentifiers", true);
            IncludeUnusedBlocks = GetConfigValue(appSettings, "IncludeUnusedBlocks");

            var functionNames = appSettings[ConfigKeyPrefix + "BlockNameProperties"];
            if (functionNames != null)
            {
                BlockNameProperties = functionNames.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            }
            else
            {
                BlockNameProperties = ComposerImportOptions.Default.BlockNameProperties;
            }
        }

        private static bool GetConfigValue(NameValueCollection appSettings, string key, bool defaultValue = false)
        {
            bool result;
            if (bool.TryParse(appSettings[ConfigKeyPrefix + key], out result))
            {
                return result;
            }
            return defaultValue;
        }

        public IEnumerable<string> BlockNameProperties { get; private set; }
        public bool Enabled { get; private set; }
        public bool TransformPageTypeIdentifiers { get; private set; }
        public bool IncludeUnusedBlocks { get; private set; }
        public bool ForceLegacyPropertyWrapper { get; private set; }
        public bool UseNullableValueTypes { get; private set; }
        public bool IncludeGroupNameInNamespace { get; set; }
        public string PageNamespace { get; set; }
        public string PageSuffix { get; set; }
        public string BlockNamespace { get; set; }
        public string BlockSuffix { get; set; }
    }
}
