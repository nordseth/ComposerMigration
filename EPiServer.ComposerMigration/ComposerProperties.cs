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
    public static class ComposerProperties
    {
        public const string Page = "ExtensionPageProperty";
        public const string ContainerPage = "ContainerPageProperty";
        public const string ContentFunction = "ExtensionContentFunctionProperty";
        public const string PersonalizationData = "PersonalizationData";
        public const string NeverUsed = "NeverUsedProperty";
    }

    public static class ComposerPropertyTypes
    {
        public const string ContentArea = "Dropit.Extension.SpecializedProperties.ExtensionContentAreaProperty";
        public const string FunctionPicker = "Dropit.Extension.SpecializedProperties.FunctionPickerProperty";
        public const string DynamicContent = "Dropit.Extension.SpecializedProperties.ExtensionDynamicContentProperty";
    }

    public static class ComposerPageTypes
    {
        public const string Prefix = "[ExtensionSys] ";
        public const string GlobalFunctionEditPage = "Global Function Edit page";
        public const string Container = "Extension Container";
        public const string PersonalizationContainer = "Personalization Container";
    }

}
